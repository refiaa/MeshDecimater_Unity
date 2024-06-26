<div align="center">

# Decimater For Unity

<em><h5 align="center">(using Unity 2022.3.22f1)</h5></em>

![img](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/e4747f15-c537-4d83-a6d1-5e69100c244c)

| **English** | [日本語](./README.jp.md) |

This plugin enables a functionality similar to Blender's decimate feature within Unity.

decimate functions are created by using **UnityMeshSimplifier**[[1]][UnityMeshSimplifier_github]

<div align="left">

### Installation
---

**How to Install**

1.  Download the file from [here][download_link] and extract it into your Assets folder.
  
2.  Next, download the latest release file (Unitypackage) from [here][download_link2] and import it to complete the setup.

**File Tree Structure**

tree have to looks like this

```shell
Assets
├─MeshDecimater_Unity
│  ├─Material
│  ├─Shader
│  └─src
└─UnityMeshSimplifier
    ├─.circleci
    │  ├─ProjectSettings
    │  └─scripts
    ├─.github
    │  ├─ISSUE_TEMPLATE
    │  └─workflows
    ├─Editor
    ├─Runtime
    │  ├─Components
    │  ├─Exceptions
    │  ├─Internal
    │  ├─Math
    │  └─Utility
    └─Tests
        └─Editor
```

### How to use
---
![src](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/1830fee5-2ae0-49d0-bac4-929a3e42ab4a)

Select an object with a mesh from the Hierarchy or choose it from the ObjectField to use the plugin.

Similar to Blender, adjust the `Decimate Level` and press `Apply Decimation` to execute the decimation.

Clicking `Revert` will restore the original file.

Please note that Revert will not work after clicking on a different object (the original mesh will remain, so you can replace it to restore).

```
work confirmed in

・Unity 2022.3.22f1

・Unity 2019.4.31f1
```

<!-- links -->
  [UnityMeshSimplifier_github]: https://github.com/Whinarn/UnityMeshSimplifier
  [download_link]: https://github.com/Whinarn/UnityMeshSimplifier/releases/tag/v3.1.0
  [download_link2]: https://github.com/refiaa/MeshDecimater_Unity/releases/latest

