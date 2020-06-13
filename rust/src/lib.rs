use std::cmp::Ordering;
use self::PokerHand::*;
use self::Suit::*;
use std::collections::BTreeMap;

pub type Cards = Vec<(usize, Suit)>;

#[derive(Debug)]
pub enum Error {
    RankNotFound,
    SuitNotFound,
}

#[derive(Debug, Eq, PartialEq)]
pub enum PokerHand {
    RoyalFlush,
    StraightFlush,
    FourOfAKind,
    FullHouse,
    Flush,
    Straight,
    ThreeOfAKind,
    TwoPair,
    OnePair,
    HighCard,
}

impl PartialOrd for PokerHand {
    fn partial_cmp(&self, other: &PokerHand) -> Option<Ordering> {
        Some(self.cmp(other))
    }
}

impl Ord for PokerHand {
    fn cmp(&self, other: &PokerHand) -> Ordering {
        let resp = [self, other].iter().map(|x| {
            match x {
                RoyalFlush => 9,
                StraightFlush => 8,
                FourOfAKind => 7,
                FullHouse => 6,
                Flush => 5,
                Straight => 4,
                ThreeOfAKind => 3,
                TwoPair => 2,
                OnePair => 1,
                HighCard => 0,   
            }
        }).collect::<Vec<usize>>();
        resp[0].cmp(&resp[1])
    }
}

#[derive(Debug, PartialEq)]
pub enum Suit {
    Spade,
    Heart,
    Diamond,
    Club,
}

#[derive(Debug)]
pub struct Hand<'a> {
    cards: Cards,
    consecutive: bool,
    same_suits: bool,
    rank_map: Vec<(usize, usize)>,
    original_hand: &'a str
}

impl<'a> Hand<'a> {
    pub fn new(cards: Cards, original_hand: &'a str) -> Hand {
        Self {
            original_hand,
            consecutive: Hand::consecutive(&cards),
            same_suits: Hand::same_suits(&cards),
            rank_map: Hand::count_same_rank(&cards),
            cards,
        }
    }

    pub fn consecutive(cards: &Cards) -> bool {
        let cards_length = cards.len();
        let (min_rank, _) = cards[0];
        let (max_rank, _) = cards[cards_length - 1];
        // only check if the range is correct
        if max_rank - min_rank + 1 == cards_length {
            return (0..(cards_length - 1)).fold(true, |boolean, x| {
                let (a, _) = cards[x];
                let (b, _) = cards[x + 1];
                a + 1 == b && boolean
            });
        }
        false
    }

    pub fn same_suits(cards: &Cards) -> bool {
        let cards_length = cards.len();
        (0..(cards_length - 1)).fold(true, |boolean, x| {
            let (_, a) = &cards[x];
            let (_, b) = &cards[x + 1];
            a == b && boolean
        })
    }

    pub fn count_same_rank(cards: &Cards) -> Vec<(usize, usize)> {
        cards
            .iter()
            .fold(BTreeMap::new(), |mut map, (rank, suit)| {
                map.entry(*rank).or_insert(0);
                if let Some(value) = map.get_mut(rank) {
                    *value += 1;
                }
                map
            })
            .iter()
            .map(|(a, b)| (*a, *b))
            .collect::<Vec<(usize, usize)>>()
    }

    pub fn royal_flush(&self) -> Option<PokerHand> {
        let (rank, suit) = &self.cards[0];
        match self.consecutive && self.same_suits && *rank == 10 as usize {
            true => Some(RoyalFlush),
            false => None,
        }
    }

    pub fn straight_flush(&self) -> Option<PokerHand> {
        match self.consecutive && self.same_suits {
            true => Some(StraightFlush),
            false => None,
        }
    }

    pub fn full_house(&self) -> Option<PokerHand> {
        let (_, count) = self.rank_map[0];
        match self.rank_map.len() == 2 && count == 3 as usize {
            true => Some(FullHouse),
            false => None,
        }
    }

    pub fn four_of_a_kind(&self) -> Option<PokerHand> {
        let (_, count) = self.rank_map[0];
        match self.rank_map.len() == 2 && count == 4 as usize {
            true => Some(FourOfAKind),
            false => None,
        }
    }

    pub fn flush(&self) -> Option<PokerHand> {
        match self.same_suits {
            true => Some(Flush),
            false => None,
        }
    }

    pub fn straight(&self) -> Option<PokerHand> {
        match self.consecutive {
            true => Some(Straight),
            false => None,
        }
    }

    pub fn three_of_a_kind(&self) -> Option<PokerHand> {
        let (_, count) = self.rank_map[0];
        match count == 3 as usize {
            true => Some(ThreeOfAKind),
            false => None,
        }
    }

    pub fn two_pair(&self) -> Option<PokerHand> {
        let (_, count) = self.rank_map[0];
        match self.rank_map.len() == 3 && count == 2 {
            true => Some(TwoPair),
            false => None,
        }
    }

    pub fn one_pair(&self) -> Option<PokerHand> {
        let (_, count) = self.rank_map[0];
        match self.rank_map.len() == 4 && count == 2 {
            true => Some(OnePair),
            false => None,
        }
    }

    pub fn evaluate(&self) -> PokerHand {
        let funcs: Vec<Box<Fn() -> Option<PokerHand>>> = vec![
            Box::new(|| self.royal_flush()),
            Box::new(|| self.straight_flush()),
            Box::new(|| self.four_of_a_kind()),
            Box::new(|| self.full_house()),
            Box::new(|| self.flush()),
            Box::new(|| self.straight()),
            Box::new(|| self.three_of_a_kind()),
            Box::new(|| self.two_pair()),
            Box::new(|| self.one_pair()),
        ];
        match funcs.iter().filter_map(|func| func()).next() {
            Some(poker_hand) => poker_hand,
            None => HighCard,
        }
    }
}

pub fn parse_hand<'a>(hand: Vec<String>) -> Cards {
    hand.iter()
        .map(|card| {
            let formatted_card = format!("{:0>3}", card);
            let chars: Vec<char> = formatted_card.chars().collect();
            let suit = match chars.last() {
                Some('S') => Ok(Spade),
                Some('H') => Ok(Heart),
                Some('D') => Ok(Diamond),
                Some('C') => Ok(Club),
                _ => Err(Error::SuitNotFound),
            };
            let rank = match [chars[0], chars[1]] {
                ['0', '1'] => Ok(1),
                ['0', '2'] => Ok(2),
                ['0', '3'] => Ok(3),
                ['0', '4'] => Ok(4),
                ['0', '5'] => Ok(5),
                ['0', '6'] => Ok(6),
                ['0', '7'] => Ok(7),
                ['0', '8'] => Ok(8),
                ['0', '9'] => Ok(9),
                ['1', '0'] => Ok(10),
                ['0', 'J'] => Ok(11),
                ['0', 'Q'] => Ok(12),
                ['0', 'K'] => Ok(13),
                ['0', 'A'] => Ok(14),
                _ => Err(Error::RankNotFound),
            };
            (rank.unwrap(), suit.unwrap())
        })
        .collect::<Cards>()
}

pub fn sort_hand(mut hand: Cards) -> Cards {
    hand.sort_by(|(a, _), (b, _)| a.cmp(b));
    hand
}

pub fn winning_hands<'a>(hands: &[&'a str]) -> Option<Vec<&'a str>> {
    let mut parsed_hands = hands.iter().map(|hand| {
        let p = hand
            .split(' ')
            .map(|b| b.to_string())
            .collect::<Vec<String>>();
        
        let resp = sort_hand(parse_hand(p));
        let hand = Hand::new(resp, hand);
        hand
    }).collect::<Vec<Hand>>();

    parsed_hands.sort_by(|a, b| {
        a.evaluate().cmp(&b.evaluate())
    });

    match parsed_hands.first() {
        Some(hand) => Some(vec![hand.original_hand]),
        None => None
    }
}

