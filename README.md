# Struct as a key in a dictionary

### Access
| Scenario       | Mono | IL2CPP |
|----------------|------|--------|
| With IEquatable| 0B   | 0B     |
| No IEquatable  | 60B  | 20B    |

# Sparse vs Dense hash map for metadata in IL2CPP

Details: https://gamedev.center/instantly-boost-unity-game-performance-with-il2cpp_use_sparsehash/

### No reflection baking
26250 classes and bindings 

#### IL2CPP_USE_SPARSEHASH 1
| Total Time (ms) |
|-----------------|
| 5301.64         |
| 5130.90         |
| 5045.09         |

#### IL2CPP_USE_SPARSEHASH 0
| Total Time (ms) |
|-----------------|
| 2251.10         |
| 2203.91         |
| 2205.35         |

### Reflection baking
26250 classes and bindings 

#### IL2CPP_USE_SPARSEHASH 1
| Total Time (ms) |
|-----------------|
| 4430.24         |
| 4488.11         |
| 4473.64         |


#### IL2CPP_USE_SPARSEHASH 0
| Total Time (ms) |
|-----------------|
| 1741.95         |
| 1751.57         |
| 1752.00         |

## How To
1. Open the "SampleScene"
2. Click "Tools/Generate" to generate test classes, including TestInstaller.cs
3. Select "TestInstaller" game object in the opened scene
4. Add TestInstaller.cs as a component
5. (Optional) Modify the count value in the SparseVsDenseHashSet.ClassGenerator.GenerateClasses() method to set the number of classes to generate.
