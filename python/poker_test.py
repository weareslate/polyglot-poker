import unittest

from poker import best_hands

# Tests adapted from `problem-specifications//canonical-data.json` @ v1.1.0


class PokerTest(unittest.TestCase):
    def test_high_card(self):
        self.assertEqual(best_hands(["4D 5S 6S 8D 3C", "2S 4C 7S 9H 10H", "3S 4S 5D 6H JH"]), ["3S 4S 5D 6H JH"])

    def test_one_pair_beats_high_card(self):
        self.assertEqual(best_hands(["4S 5H 6C 8D KH", "2S 4H 6S 4D JH"]), ["2S 4H 6S 4D JH"])
    
    def test_three_of_a_kind_beats_two_pair(self):
        self.assertEqual(best_hands(["2S 8H 2H 8D JH", "4S 5H 4C 8S 4H"]), ["4S 5H 4C 8S 4H"])
    
    def test_straight_beats_three_of_a_kind(self):
        self.assertEqual(best_hands(["4S 5H 4C 8D 4H", "3S 4D 2S 6D 5C"]), ["3S 4D 2S 6D 5C"])
    
    def test_flush_beats_straight(self):
        self.assertEqual(best_hands(["4C 6H 7D 8D 5H", "2S 4S 5S 6S 7S"]), ["2S 4S 5S 6S 7S"])

    def test_full_house_beats_flush(self):
        self.assertEqual(best_hands(["3H 6H 7H 8H 5H", "4S 5C 4C 5D 4H"]), ["4S 5C 4C 5D 4H"])

    def test_four_kind_beats_full_house(self):
        self.assertEqual(best_hands(["4S 5H 4D 5D 4H", "3S 3H 2S 3D 3C"]), ["3S 3H 2S 3D 3C"])

    def test_straight_flush_beats_four_kind(self):
        self.assertEqual(best_hands(["4S 5H 5S 5D 5C", "7S 8S 9S 6S 10S"]), ["7S 8S 9S 6S 10S"])

    def test_royal_flush_beats_straight_flush(self):
        self.assertEqual(best_hands(["10S JS QS KS AS", "7S 8S 9S 6S 10S"]), ["10S JS QS KS AS"])
if __name__ == "__main__":
    unittest.main()

