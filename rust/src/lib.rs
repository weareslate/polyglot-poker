use std::cmp::Ordering;
use std::collections::BTreeMap;
use std::collections::BTreeSet;
use std::iter::FromIterator;

pub fn pad_vec(vec: Vec<usize>) -> Vec<usize> {
    Vec::from(&[vec, vec![1; 5]].concat()[0..5])
}

pub fn consecutive(ranks: Vec<usize>) -> bool {
    if ranks.len() != 5 {
        return false;
    }
    let lo = ranks.first().unwrap();
    let hi = ranks.last().unwrap();
    let sum: usize = ranks.iter().sum();
    2.5 * (lo + hi) as f32 == sum as f32
}

pub fn parse<'a>(hand: &'a str) -> (BTreeSet<String>, Vec<usize>, Vec<usize>) {
    let mut suits: BTreeSet<String> = BTreeSet::new();
    let mut ranks: BTreeMap<usize, usize> = BTreeMap::new();
    for card in hand.split(' ') {
        let chars: Vec<char> = format!("{:0>3}", card).chars().collect();
        let rank = match String::from_iter([chars[0], chars[1]].iter()).as_ref() {
            "0J" => 11,
            "0Q" => 12,
            "0K" => 13,
            "0A" => 14,
            c => c.parse::<usize>().unwrap(),
        };
        suits.insert(chars[2].to_string());
        *ranks.entry(rank).or_insert(0) += 1;
    }
    let (ranks, mut freq): (Vec<usize>, Vec<usize>) = ranks.iter().unzip();
    freq.sort();
    freq.reverse();
    let freq = pad_vec(freq)
        .iter()
        .map(|x| x.clone())
        .collect::<Vec<usize>>();
    (suits, ranks, freq)
}

pub fn hand_score<'a>(hand: &'a str, index: usize) -> (Vec<usize>, usize) {
    let (suits, ranks, mut freq) = parse(hand);
    let high_card = *ranks.last().unwrap();
    if suits.len() == 1 {
        freq[0] = freq[0] + 2;
        freq[1] = freq[1] + 2;
    }
    if consecutive(ranks) {
        freq[0] = freq[0] + 2;
        freq[1] = freq[1] + 1;
    }
    freq.push(high_card);
    (freq, index)
}

pub fn winning_hands<'a>(hands: &[&'a str]) -> Option<Vec<&'a str>> {
    let mut ev_hands = hands
        .iter()
        .enumerate()
        .map(|(i, hand)| hand_score(hand, i))
        .collect::<Vec<(Vec<usize>, usize)>>();
    ev_hands.sort_by(|a, b| b.0.cmp(&a.0));
    let (_, i) = ev_hands[0];
    Some(vec![hands[i]])
}

pub fn main() {
    // TODO: handle multiple winners
    let resp = winning_hands(&["10D JD KD QD AD", "2S 4C 4S 4H 4D", "3S 4S 5D 6H JH"]);
    assert_eq!(resp, &["10D JD KD QD AD"]);
}
