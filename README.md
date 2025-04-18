# UzumoreShader 
埋もれシェーダーはカメラがメッシュの内側に潜り込んだ際に頂点をカメラの正面まで押し戻すことでメッシュの内側を見えなくするシェーダーです。
liltoonのカスタムシェーダー機能を用いて作成されています。

## 導入
[ミルティナちゃん](https://booth.pm/ja/items/6538026)を例に導入の手順を説明していきます。

### 1.プロジェクトに埋もれシェーダーを追加する
1. [こちら](https://sigmal00.github.io/UzumoreShaderVPM/)のURLにアクセスし "Add To VCC" を選択してVCCに追加

![image](https://github.com/user-attachments/assets/7900e8c0-5613-4d8c-9065-1f4fe16fee54)

2. VCCを開き、シェーダーを追加したいプロジェクトの"Manage Project"を選択

![image](https://github.com/user-attachments/assets/1004759c-0bff-49ef-8b4a-d9a3f2eeef8d)

3. パッケージ一覧から"UzumoreShader"を探してプロジェクトに追加する。

![image](https://github.com/user-attachments/assets/8469c2cc-aeb3-4d79-946e-7fdbd92d4600)

4. liltoonのバージョンが1.8.5以前の場合はバージョンアップが要求されるので"Confirm"を選択します。

![image](https://github.com/user-attachments/assets/c4d2d291-a34a-4906-8b8b-8c06dfb773a4)

5. プロジェクトを開き、適当なマテリアルのシェーダー選択欄から"Sigmal00/Uzumore/..."以下のシェーダーが選択できるようになっていることを確認します。

![image](https://github.com/user-attachments/assets/0e4ffb80-f33c-4012-a9bd-30ecc41b566f)


<details><summary>unitypackage版から移行する場合はこちら</summary>

プロジェクトにすでに[unitypackage版の埋もれシェーダー](https://github.com/sigmal00/UzumoreShader)をインポートしている方向けの手順です。
1. [こちら](https://sigmal00.github.io/UzumoreShaderVPM/)のURLにアクセスし "Add To VCC" を選択してVCCに追加
2. プロジェクトを閉じてからエクスプローラーなどで"Assets\Sigmal00\UzumoreShader"以下のフォルダを削除する。
3. VCCを開き、シェーダーを追加したいプロジェクトの"Manage Project"を選択
4. パッケージ一覧から"UzumoreShader"を探してプロジェクトに追加する。
5. liltoonのバージョンが1.8.5以前の場合はバージョンアップが要求されるので"Confirm"を選択します。
6. プロジェクトを開き、適当なマテリアルのシェーダー選択欄から"Sigmal00/Uzumore/..."以下のシェーダーが選択できるようになっていることを確認します。

</details>

### 2.既存のliltoonマテリアルに埋もれシェーダーを適用する
#### ver1.0.12以降
既にliltoonシェーダーのパラメータを設定済みのマテリアルがある場合は、**マテリアルを右クリックして"UzumoreShader/Convert material to UzumoreShader"を選択**すればマテリアルを埋もれシェーダーに対応したものに変換できます。
* **変換前にマテリアルを複製するなどしてバックアップを取っておくことをおすすめします**

![image](https://github.com/user-attachments/assets/a1140e74-33c9-45d0-867c-d4834ef0a5a7)

**<details><summary>ver1.0.11以前</summary>**

既にliltoonシェーダーのパラメータを設定済みのマテリアルがある場合は、マテリアルのシェーダー選択欄からシェーダーを埋もれシェーダーに切り替えれば、パラメータを（ほぼ全て）維持したまま埋もれシェーダーに切り替えることができます。

![image](https://github.com/user-attachments/assets/4ec84fb5-3490-4810-a31f-575425d8808f)

輪郭線設定をしていた場合、輪郭線設定のチェックが外れてしまうので再度チェックを入れなおしてください。

![image](https://github.com/user-attachments/assets/5e385bc0-5965-4d73-b914-aacd6349c789)

描画モードを半透明やカットアウトなどに設定していた場合も描画モードが不透明に切り替わってしまうので、その場合も描画モードを再度切り替え直してください。

![image](https://github.com/user-attachments/assets/9e605758-5b75-479f-9951-b1ecf43a5c11)

</details>

## パラメータ説明
### どこまで埋もれるか（ｍ）
カメラがメッシュの内側に潜り込んだとき、何メートルまで押し戻してくれるかを設定します。

値が大きすぎると意図しない場面で埋もれが発生することがあるので、0.1～0.2くらいにしたほうが無難です。

### 埋もれマスク
埋もれが有効になる範囲をマスクテクスチャで設定できます。

テクスチャの白い部分で埋もれが有効になり、黒い部分で埋もれが無効になります。

#### 体のメッシュのうち手だけ埋もれさせたい場合の設定例
肌のテクスチャを元に、以下のように手の部分を白く、それ以外を黒く塗ったテクスチャを用意します。

![Milltina_Body](https://github.com/user-attachments/assets/fe64c5f0-a3f6-44dd-b13c-d6867b7d3c6b)

用意したテクスチャを**埋もれマスク**に設定します。

![image](https://github.com/user-attachments/assets/4448fe92-4278-4fcc-82df-ea3fe45bd3f1)


### 埋もれバイアス（ｍ）
#### ver1.0.1以降
Z-Fightを防ぐためのパラメータです。
Z-Fightを起こす（メッシュがチラつく）場合にはこの値を大きくしてみてください。

**<details><summary>ver1.0.0</summary>**

頂点の押し戻しが始まる距離に対するバイアス値です。

埋もれシェーダーを適用したメッシュ同士が重なってZ-Fightを起こす場合はこの値を調整してください。

その際は内側にあるメッシュは外側にあるメッシュよりもこの値を大きく設定してください。

例として、服メッシュのバイアス値が0.001のときは肌メッシュのバイアス値を0.002に設定すると良いでしょう。

</details>

## Q&A
### マテリアルのシェーダーを埋もれシェーダーに切り替えたら頬染めがバグった・メッシュが透過されなくなった
マテリアルの描画モードが半透明から不透明に戻ってしまっています。マテリアルの描画モードを「不透明」から「半透明」に変更してください。

### しゃがんだときや下を向いたときに視界の下の方に肌のようなものが見える
首のメッシュが押し戻されて映り込んでしまっています。

**埋もれマスク**に首元を黒くマスクしたテクスチャを設定してみてください。

また、VRChatの設定のGraphicsのページにある**Forced Camera Near Distance**をDynamicやForcedに設定すると解決することがあります（基本的にはマスクの設定をオススメします）

### 服に埋もれシェーダー適用して埋もれたら肌メッシュが貫通してきた
肌にも埋もれシェーダーを適用してください。

### 埋もれても暗くならない
liltoonの距離フェードを設定してください。

### 人の背後に立ったら相手の視界が塞がってしまった
おそらく背中のメッシュが相手の眼前まで押し戻されています。

**どこまで埋もれるか**の値を小さくするか、**埋もれマスク**に埋もれさせたい範囲をマスクしたテクスチャを設定してみてください。
* 一般的な身長のアバターであれば**どこまで埋もれるか**の値は0.1~0.2あたりに設定するのをオススメします。
