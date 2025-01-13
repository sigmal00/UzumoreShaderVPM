# UzumoreShader 
埋もれシェーダーはカメラがメッシュの内側に潜り込んだ際に頂点をカメラの正面まで押し戻すことでメッシュの内側を見えなくするシェーダーです。

## 導入
WIP

## パラメータ説明
### どこまで埋もれるか（ｍ）
カメラがメッシュの内側に潜り込んだとき、何メートルまで押し戻してくれるかを設定します。

### 埋もれマスク
押し戻してくれる範囲のマスクです。

どこまで埋もれるかの値にRチャンネルの値が乗算されます。

### 埋もれバイアス（ｍ）
頂点の押し戻しが始まる距離に対するバイアス値です。

埋もれシェーダーを適用したメッシュ同士が重なってZ-Fightを起こす場合はこの値を調整してください。

その際は内側にあるメッシュは外側にあるメッシュよりもこの値を大きく設定してください。

例として、服メッシュのバイアス値が0.001のときは肌メッシュのバイアス値を0.002に設定すると良いでしょう。

## Q&A
### 服に埋もれシェーダー適用して埋もれたら肌メッシュが貫通してきた
肌にも埋もれシェーダーを適用してください。

### 埋もれても暗くならない
liltoonの距離フェードを設定してください。
