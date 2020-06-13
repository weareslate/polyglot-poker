# Polyglot Poker

Welcome to polyglot poket. Much like the MVC todo apps this is a repo where the rules of poker hands has been implemented in many different langugaes.

## Challenge

Write a programme that takes and array of poker hands and returns an array of winning hands. Input poker hands should be formatted as an array of strings like this:

```
["4S 4H 4D 4C AS", "JS KD QC 10H 2C"]
```

If (above) is your input then the return value of your function should be

```
["4S 4H 4D 4C AS"]
```

because this is the winning hand with four of a kind.

## Tests

#### single hand
input
`["4S 5S 7H 8D JC"]`
output
`["4S 5S 7H 8D JC"]`


#### high card
input:
```
["4D 5S 6S 8D 3C", "2S 4C 7S 9H 10H", "3S 4S 5D 6H JH"]
```
output:
```
["3S 4S 5D 6H JH"]
```

#### multiple winners
input:
```
["4D 5S 6S 8D 3C", "2S 4C 7S 9H 10H", "3S 4S 5D 6H JH", "3H 4H 5C 6C JD",]
```
output:
```
["3S 4S 5D 6H JH", "3H 4H 5C 6C JD]"
```

#### one pair beats high card
input:
```
["4S 5H 6C 8D KH", "2S 4H 6S 4D JH"]
```
output:
```
["2S 4H 6S 4D JH"]
```
#### two pair beats one pair
input:
```
["2S 8H 6S 8D JH", "4S 5H 4C 8C 5C"]
```
output: 
```
["4S 5H 4C 8C 5C"]
```

#### three of a kind beats two pair
input: 
```
["2S 8H 2H 8D JH", "4S 5H 4C 8S 4H"]
```
output:
```
["4S 5H 4C 8S 4H"]
```

#### straight beats three of a kind
input: 
```
["4S 5H 4C 8D 4H", "3S 4D 2S 6D 5C"]
```
output:
```
["3S 4D 2S 6D 5C"]
```

#### flush beats straight
input: 
```
["4C 6H 7D 8D 5H", "2S 4S 5S 6S 7S"]
```
output:
```
["2S 4S 5S 6S 7S"]
```

#### full house beats flush
input: 
```
["3H 6H 7H 8H 5H", "4S 5C 4C 5D 4H"]
```
output:
```
["4S 5C 4C 5D 4H"]
```

#### four of a kind beats full house
input: 
```
["4S 5H 4D 5D 4H", "3S 3H 2S 3D 3C"]
```
output:
```
["3S 3H 2S 3D 3C"]
```

#### straight flush beats four of a kind
input: 
```
["4S 5H 5S 5D 5C", "7S 8S 9S 6S 10S"]
```
output:
```
["7S 8S 9S 6S 10S"]
```

#### royal flush beats lower straight flush
input: 
```
["10S JS QS KS AS", "7S 8S 9S 6S 10S"]
```
output:
```
["10S JS QS KS AS"]
```

## Bonus tests

- Test that A can be at the start of end of a straight example end: `10 J Q K A` example start would be `A 2 3 4 5`
