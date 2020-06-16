ranks_list = ['2', '3', '4', '5', '6', '7', '8', '9', '10', 'J', 'Q', 'K','A']

def parse_hand(hand):
    parsed = [(card[:-1], card[-1:]) for i, card in enumerate(hand.split(' '))]
    ranks_count = {rank: ''.join(hand).count(rank) for rank, _ in parsed}
    counts, ranks = list(zip(*sorted([(count, ranks_list.index(rank)) for rank, count in ranks_count.items()])[::-1]))

    if len(counts) == 5:
        if ranks[0:2] == (12, 3): 
            ranks = (3, 2, 1, 0, -1)
        straight = ranks[0] - ranks[4] == 4
        flush = len({ suit for _, suit in parsed }) == 1
        
        if straight and flush:
            counts = (4, 2, 1, 1, 1 + ranks[-1:][0])
        elif straight:
            counts = (3, 1, 1, 2, 1)
        elif flush:
            counts = (3, 1, 2, 1, 1)
    
    if counts == (1,1,1,1,1):
        counts = (1,1,1,1,ranks[-1:][0])
    
    return counts, ranks

def best_hands(hands):
    resp = [(i, parse_hand(hand)) for i, hand in enumerate(hands)]
    winner = sorted(resp, key=lambda x: x[1], reverse=True)[0][0]
    return [hands[winner]]

