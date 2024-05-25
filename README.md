# Struct as a key in a dictionary

### Access
| Scenario       | Mono | IL2CPP |
|----------------|------|--------|
| With IEquatable| 0B   | 0B     |
| No IEquatable  | 60B  | 20B    |
