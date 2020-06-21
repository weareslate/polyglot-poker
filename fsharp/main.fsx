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

type Suit = Hearts | Spades | Diamonds | Clubs

type Card = Face * Suit

type WinningHands =
  | HighCard
  | Pair
  | TwoPair
  | ThreeOfAKind
  | Straight
  | Flush
  | FullHouse
  | FourOfAKind
  | StraightFlush
  | RoyalFlush

type Hand = Card list

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
  Seq.foldBack (Option.map2 (fun x y -> x::y)) list (Some [])

let parseHand (hand: string) =
  hand.Split([|' '|], StringSplitOptions.RemoveEmptyEntries) 
  |> Seq.map parseCard
  |> optionFoldMap

let understandHands (hands: string) = 
  hands.Split([|';'|], StringSplitOptions.RemoveEmptyEntries) 
  |> Seq.map parseHand
  |> optionFoldMap

let allFaces = [ Two; Three; Four; Five; Six; Seven; Eight; Nine; Ten; Jack; Queen; King; Ace]

// imagine a pairwise [1;2;3;4;5] -> [(1,2);(2,3);(3,4);(4,5)]
// but doesn't have to be a pair, so 5 instead [1;2;3;4;5;6] -> [[1;2;3;4;5];[2;3;4;5;6]]
let elementwise (list: 'T list) elementCount  =
  let mutable ans = []
  for i in 1..list.Length do
    try ans <- ans @ [(list |> List.skip (i-1) |> List.take elementCount)]
    with _ -> ()
  ans

let straightRuns = elementwise allFaces 5

let ofAKind groupByFn conditionPair cards =
  cards |> List.groupBy groupByFn 
  |> List.map (fun (_,values) -> values |> List.length) |> List.countBy id 
  |> List.max |> Some |> Option.filter (fun maxPair -> maxPair = conditionPair) 
// before we were doing match something with (3,1) -> Some answer. basically if it is the right pair then return some
// we can shorten that match down, by converting output to an option, then filtering based on whether the pair
// matches the input pair condition 

let (| RoyalFlush | _ |) (cards: Card list) =
  let highestStraight = [[Ace;King;Queen;Jack;Ten]] |> List.contains (cards |> List.map fst)
  let allSuits = cards |> ofAKind snd (5,1)
  if highestStraight && allSuits.IsSome then
    Some cards
  else
    None

let (| Straight | _ |) (cards: Card list) =
  straightRuns |> List.contains (cards |> List.map fst) |> Some |> Option.filter id

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

let (| HighCard |) (cards: Card list) =
  cards |> ofAKind fst (1,5)

let handInput = fsi.CommandLineArgs.[1]

let theTwoHands = understandHands handInput

let obtainHand = function // this is a match...with expression, but because it assumes its taking first arg, we just shorten to function
  | RoyalFlush cards -> RoyalFlush
  | Straight _ & Flush cards -> StraightFlush
  | FourOfAKind cards-> FourOfAKind
  | Flush cards -> Flush
  | Straight cards -> StraightFlush
  | ThreeOfAKind _ & Pair cards -> FullHouse
  | ThreeOfAKind cards -> ThreeOfAKind
  | TwoPair cards -> TwoPair
  | Pair cards -> Pair
  | HighCard cards -> HighCard

match theTwoHands with
| Some hands -> 
    let firstHand = hands.[0]
    let secondHand = hands.[1]
    let whoWon = obtainHand firstHand  > obtainHand secondHand
    if whoWon then
      printfn "Player 1, with %A\nPlayer 2 had %A" (obtainHand firstHand) (obtainHand secondHand)
    else 
      printfn "Player 2, with %A\nPlayer 1 had %A" (obtainHand secondHand) (obtainHand firstHand)
| None -> printfn "No combinations"
