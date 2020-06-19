open System;

type Face =
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
  | Ace

type Suit =
  | Hearts
  | Spades
  | Diamonds
  | Clubs

type Card = Face * Suit

type Hand = Card list

let bindMap f b a =
  a |> Option.bind ( fun realA ->
    b |> Option.map ( fun realB ->
      f realA realB
  ))

let parsingFaceMap =
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

      let tryFirst = parsingFaceMap.TryFind first // option face
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

  parsedHands

let allFaces = [ Two; Three; Four; Five; Six; Seven; Eight; Nine; Ten; Jack; Queen; King; Ace]

// imagine a pairwise [1;2;3;4;5] -> [(1,2);(2,3);(3,4);(4,5)]
// but doesn't have to be a pair, so 5 instead [1;2;3;4;5;6] -> [[1;2;3;4;5];[2;3;4;5;6]]
let elementwise (list: 'T list) elementCount  =
  let mutable ans = []
  for i in 1..list.Length do
    try 
      ans <- ans @ [(list |> List.skip (i-1) |> List.take elementCount)]
    with 
    | _ -> ()
  ans

let straightRuns = elementwise allFaces 5

let ofAKind groupByFn a cards =
  cards |> List.groupBy groupByFn 
  |> List.map (fun (_,values) -> values |> List.length) |> List.countBy id 
  |> List.max |> Some |> Option.filter (fun p -> p = a) 
// before we were doing match something with (3,1) -> Some answer. basically if it is the right pair then return some
// we can shorten that match down, by converting output to an option, then filtering based on whether the pair
// matches the input pair condition 

let (| Straight | _ |) (cards: Card list) =
  straightRuns |> List.contains (cards |> List.map fst) |> function
  | true -> Some cards
  | false -> None

let (| Flush | _ |) (cards: Card list) =
  cards |> ofAKind snd (5,1)

let (| FourOfAKind | _ |) (cards: Card list) =
  cards |> ofAKind fst (4,1)
 
let (| ThreeOfAKind | _ |) (cards: Card list) =
  cards |> ofAKind fst (3,1)

let (| TwoPair | _ |) (cards: Card list) = 
  cards |> ofAKind fst (2,2)

let (| Pair | _ |) (cards: Card list) =
  cards |> ofAKind fst (2,1)

let (| HighCard | _ |) (cards: Card list) =
  cards |> ofAKind fst (1,5)

let handInput = fsi.CommandLineArgs.[1]

let theTwoHands = understandHands handInput

let displayHand = function // this is a match...with expression, but because it assumes its taking first arg, we just shorten to function
  | Straight _ & Flush cards -> printfn "Have straight flush"
  | Flush cards -> printfn "Have flush"
  | Straight cards -> printfn "Have straight"
  | FourOfAKind cards-> printfn "Have four of kind"
  | ThreeOfAKind cards -> printfn "Have three of kind"
  | TwoPair cards -> printfn "Have two pair"
  | Pair cards -> printfn "Have pair"
  | HighCard cards -> printfn "Have high card"
  | _ -> printfn "You have no cards"

match theTwoHands with
| Some hands -> 
    let firstHand = hands.[0]
    displayHand firstHand 
    let secondHand = hands.[1]
    displayHand secondHand
| None -> printfn "No combinations"
