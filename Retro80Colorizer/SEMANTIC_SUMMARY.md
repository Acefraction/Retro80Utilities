
# セマンティック減色手法の要約

## 1. 数学的基礎

セマンティックラベル（例：「街並み・昼」「キャラ緑」など）は、**Lab色空間上の関数**として表現されます。  
各セマンティックは、代表的なLab点群から推定される関数 `f_semantic(L, a, b)` に対応します。

使用可能な関数モデル：

- 凸包（Convex Hull）
- ガウス混合モデル（GMM）
- カーネル密度推定（KDE）
- k近傍法（k-NN）

---

## 2. セマンティック領域の重なり検出

Lab空間では、複数のセマンティック領域が重なる可能性があります。  
その重なりを定量的に評価する方法：

- **共通サポート領域**： `f_i > 0` かつ `f_j > 0` となる領域
- **確率的重なり評価**： Bhattacharyya距離や最小値積分

---

## 3. 初期セマンティック領域の割り当て

縮小画像に対してセマンティックを初期割当するには、**モンテカルロ法的シミュレーション**を用います。

処理フロー：

1. ランダムにピクセルを選び、Lab空間にノイズを加える  
2. 周囲のラベル付きピクセルからの「引力」によって微小変動  
3. 得られたLab値を、**体積が最小のセマンティック領域**にマッピング  
4. ノイズ幅を徐々に下げながら繰り返す

---

## 4. セマンティックディノイズ処理

初期ラベルの後、小さな飛び地的領域は「意味ノイズ」と見なして吸収処理を行います。

- `S_n`（しきい値）未満の連結領域を検出
- 接触しているセマンティックで最も接触面積が大きいものに統合

この処理で意味的な統一感を維持できます。

---

## 5. 実装可能性と応用

セマンティック関数、確率的初期化、ノイズ吸収の三段階を持つことで、  
**意味を考慮した減色システム**（Semantic-aware Dithering）が構築可能です。

---

### 🔍 注記

- Lab空間ではセマンティック関数は重なり得るが、画像ピクセルには**排他的ラベル**が必要
- 本手法は、**確率的意味ラベリング + 局所引力場によるエネルギー最小化**を行う構造を持ちます

---

# Summary of Semantic Color Reduction Methods

## 1. Mathematical Foundation

Semantic labels (e.g., "urban daytime", "character green") are represented as functions over the Lab color space.
Each semantic corresponds to a function `f_semantic(L, a, b)` whose domain is inferred from a set of representative Lab points.

Modeling methods include:

- Convex Hull
- Gaussian Mixture Models (GMM)
- Kernel Density Estimation (KDE)
- k-Nearest Neighbors

---

## 2. Semantic Overlap Detection

Multiple semantic domains can coexist in Lab space. Their overlap can be evaluated using:

- Shared support: regions where `f_i > 0` and `f_j > 0`
- Probability overlap: Bhattacharyya distance or minimum value integrals

---

## 3. Initial Semantic Region Assignment

For downscaled images, semantic labels can be assigned through a Monte Carlo simulation.

Procedure:

1. Randomly select a pixel and apply Lab-space noise  
2. Apply local attraction toward neighboring labeled pixels  
3. Map to the semantic region with the smallest enclosing volume  
4. Repeat with gradually decreasing noise

---

## 4. Semantic Denoising

Small disconnected regions are treated as semantic noise:

- Detect connected components smaller than threshold `S_n`
- Absorb them into the most frequent neighboring semantic region

This maintains semantic consistency across the image.

---

## 5. Implementation Readiness

With semantic functions, probabilistic initialization, and noise absorption,
a full-featured **semantic-aware dithering system** can be implemented.

---

### 🔍 Notes

- Semantic functions may overlap in Lab space, but only one label can be assigned per pixel
- The algorithm minimizes semantic energy using probabilistic labeling and local attraction fields
