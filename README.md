# PlayFabBook
本リポジトリは PlayFab ゲーム開発テクニックのサンプルコードのリポジトリです。

本アプリケーションのサンプルコードはご自由にご利用ください。
ただし同梱されるアセット/ライブラリの扱いはそれぞれのライセンスを遵守してください。

## 事前準備

そのままでは正常に動作しませんので、いくつか事前準備が必要です。

### API 機能を有効にする

本来はサーバー処理とするのが望ましい API 機能について、本アプリケーションではクライアントから処理ができるようにします。  
「タイトルの設定 > API 機能」を開き、以下画像の３つのチェックボックスを ON にして保存してください。

<img width="1225" alt="playfab-book-0001000" src="https://user-images.githubusercontent.com/61415027/124339646-dc501c80-dbea-11eb-89f6-75309172feb3.png">

### タイトル ID を確認して設定する

`Assets/Scripts/Manager/LoginManager.cs`の中でタイトル ID を指定する箇所があります。  
先ほどの「API 機能を有効にする」でタイトルIDを確認し、コンストラクタのタイトル ID に値を設定してください。

```csharp
namespace PlayFabBook
{
    /// <summary>
    /// PlayFab へのログインを管理する。
    /// </summary>
    public static class LoginManager
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        static LoginManager()
        {
            // PlayFab の TitleId を指定する
            PlayFabSettings.staticSettings.TitleId = "";
        }
    }
}
```

### クライアントプロフィールオプションの設定

タイトルの設定からクライアントプロフィールオプションを開き、次の項目にチェックを付けて保存してください。

- 表示名
- 前回ログイン日時
- 統計情報

<img width="1208" alt="playfab-book-0002000" src="https://user-images.githubusercontent.com/61415027/124339743-731cd900-dbeb-11eb-8d8b-792508dfd83c.png">

### JSON ファイルのアップロード

指定のマスタデータが登録されていることを前提としているため、アップロードできる JSON ファイルを用意しました。  
`PlayFabBookSample/Sample/SampleTitle/Assets/PlayFabBook/JSON/`

#### タイトルデータのアップロード

「コンテンツ > タイトルデータ」を開き、 「JSON をアップロード」から`title-data.json`をアップロードしてください。

<img width="856" alt="playfab-book-0003000" src="https://user-images.githubusercontent.com/61415027/124339856-2dacdb80-dbec-11eb-9cae-8954973c97f4.png">

#### 仮想通貨のアップロード

「エコノミー > 通貨」を開き、 「JSON をアップロード」から`currency.json`をアップロードしてください。  
最初に開いた場合はアップロードボタンが出ないので、適当な仮想通貨の登録が必要です。

<img width="535" alt="playfab-book-0004000" src="https://user-images.githubusercontent.com/61415027/124339913-841a1a00-dbec-11eb-9df8-7a6956ff8327.png">

#### カタログのアップロード

「エコノミー > カタログ」を開き、 「JSON をアップロード」から`catalog.json`をアップロードしてください。

<img width="724" alt="playfab-book-0005000" src="https://user-images.githubusercontent.com/61415027/124339928-9a27da80-dbec-11eb-91e6-46100258a937.png">

アップロード後は、以下の手順でプライマリカタログとして設定してください。

<img width="276" alt="playfab-book-0010000" src="https://user-images.githubusercontent.com/61415027/124348133-310f8980-dc23-11eb-9eb5-32ed78b11470.png">
<img width="416" alt="playfab-book-0011000" src="https://user-images.githubusercontent.com/61415027/124348143-4389c300-dc23-11eb-901d-2d2c2d53c102.png">

#### ストアのアップロード

「エコノミー > カタログ > Main > ストア」を開き、 「JSON をアップロード」から`stores.json`をアップロードしてください。

<img width="897" alt="playfab-book-0012000" src="https://user-images.githubusercontent.com/61415027/124348294-1b4e9400-dc24-11eb-884e-4ec0d75df2c2.png">

### ランキングの登録

以下のランキングを登録してください。

- CharacterId
- CharacterLevel
- Level

<img width="616" alt="playfab-book-0006000" src="https://user-images.githubusercontent.com/61415027/124339952-b62b7c00-dbec-11eb-9743-5943733d6970.png">
<img width="608" alt="playfab-book-0007000" src="https://user-images.githubusercontent.com/61415027/124339974-d6f3d180-dbec-11eb-82e9-daa09cec7853.png">
<img width="609" alt="playfab-book-0008000" src="https://user-images.githubusercontent.com/61415027/124339978-dce9b280-dbec-11eb-919d-bea64f713b34.png">

### アクションとルールの登録

「自動化 > ルール」を開き、以下の情報で登録してください。

<img width="942" alt="playfab-book-0009000" src="https://user-images.githubusercontent.com/61415027/124340153-1ff85580-dbee-11eb-878d-99e22037513e.png">

### タイトルシーンの起動

`Assets/PlayFabBook/Scenes/Title/TitleScene.unity`を起動して、実行してください。

<img width="1011" alt="playfab-book-0013000" src="https://user-images.githubusercontent.com/61415027/124348236-be52de00-dc23-11eb-802e-18be357eaa81.png">

## 本アプリケーションで使用しているアセット/ライブラリに関するライセンス表記
PlayFab/CSharpSDK

Apache License 2.0

http://www.apache.org/licenses/LICENSE-2.0

---

UniTask

The MIT License (MIT)

Copyright (c) 2019 Yoshifumi Kawai / Cysharp, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

---

UniRx

The MIT License (MIT)

Copyright (c) 2018 Yoshifumi Kawai

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

---

JSON.NET

The MIT License (MIT)

Copyright (c) 2007 James Newton-King

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

---

本アプリケーションのフォントに「Mgen+」(http://jikasei.me/font/mgenplus/) を使用しています。
Licensed under SIL Open Font License 1.1 (http://scripts.sil.org/OFL)
© 2015 自家製フォント工房, © 2014, 2015 Adobe Systems Incorporated, © 2015 M+
FONTS PROJECT

---

本アプリケーションは「ぴぽや」(http://blog.pipoya.net/) の素材データを使用しています。

---
