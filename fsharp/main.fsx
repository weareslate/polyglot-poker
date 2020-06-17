open System;

type Rank =
  | Ace
  | Two
  | Three
  | Four
  | Five
  | Six
  | Seven
  | Eight
  | Nine
  | Ten
  | Jack
  | Queen
  | King

type Suit =
  | Hearts
  | Spades
  | Diamonds
  | Clubs

type Card = Rank * Suit

type Hand = Card list

let bindMap f b a =
  a |> Option.bind ( fun realA ->
    b |> Option.map ( fun realB ->
      f realA realB
  ))

let parsingRankMap =
  [
    ("A", Ace)
    ("K", King)
    ("Q", Queen)
    ("J", Jack)
    ("10", Ten)
    ("9", Nine)
    ("8", Eight)
    ("7", Seven)
    ("6", Six)
    ("5", Five)
    ("4", Four)
    ("3", Three)
    ("2", Two)
  ] |> Map.ofList

let parsingSuitMap =
  [
    ("H", Hearts)
    ("D", Diamonds)
    ("S", Spades)
    ("C", Clubs)
  ] |> Map.ofList

let parseCard (card: string) =
  let parts = 
    let split = card |> Seq.toList
    match card.Length with
    | 2 -> Some (split |> List.map string) // ["4";"S"]
    | 3 -> Some ([new string [|for c in split.[0..1] -> c|]; split.[2] |> string]) // ["10"; "H"]
    | _ -> None

  match parts with
  | Some parts -> 
      let first = parts.[0]
      let second = parts.[1]

      let tryFirst = parsingRankMap.TryFind first // option rank
      let trySecond = parsingSuitMap.TryFind second // option suit

      tryFirst |> Option.bind (fun first -> // real value here
        trySecond |> Option.map (fun second -> // real value 
          first,second
        ))
  | None -> None

let collectHand items =
  Option.map (fun items -> items |> List.ofSeq) items

let optionFoldMap (list: 'T option seq) =
  list 
  |> Seq.fold (fun agg elem -> 
                  agg |> Option.bind ( fun l -> 
                    elem |> Option.map ( fun card ->
                      l @ [ card ]
                    )
                  )) (Some []) 

let parseHand (hand: string) =
  let cards = hand.Split([|' '|], StringSplitOptions.RemoveEmptyEntries) 
  cards
  |> Seq.map parseCard
  |> optionFoldMap

let understandHands (hands: string) = 
  let parts = hands.Split([|';'|], StringSplitOptions.RemoveEmptyEntries) 
  let parsedHands = 
    parts
    |> Seq.map parseHand
    |> optionFoldMap

  match parsedHands with
  | Some hands -> printfn "%A" hands
  | None -> printfn "Couldn't parse the hands"


let handInput = fsi.CommandLineArgs.[1]

understandHands handInput