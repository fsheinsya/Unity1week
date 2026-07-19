# Underside

unity1week（お題「うら」）参加作品。どちらのスライムが勝つかを予想する賭けゲームです。
計3回の賭けフェーズで勝敗を予想し、最終的なコイン所持数でエンディングが分岐します。

## 公開URL

ブラウザ上でプレイできます。
[unityroomでプレイする](https://unityroom.com/games/underdside)

- 使用技術：Unity / C# / UniTask

## 概要

各ラウンドで、ランダム生成された2体のモンスター（HP・ATK・DEF・成長率・特殊能力）のどちらが勝つかを予想して賭けます。
バトルは自動で進行し、予想的中で賭け金が2倍になります。3ラウンド終了時の所持コインで4種のエンディングに分岐します。

## 設計として意識したこと

前作（毛一つ）で「共有状態の二重管理」「Git運用の崩壊」を経験した反省から、今回は構造を先に決めてから作ることをテーマにしました。柱は3つです。

### シミュレーションと演出の分離

戦闘ロジックの中核 `BattleSimulator` は、MonoBehaviourに依存しない純粋なC#クラスです。
2体のモンスターと賭け情報を受け取り、戦闘全体を一括計算して「勝敗＋戦闘ログ（文字列のリスト）」を `MatchResultData` として返します。

演出側の `BattleSceneController` は、確定済みのログをUniTaskで1行ずつ文字送り再生するだけです。

この分離により、戦闘の計算結果が演出の実装や再生タイミングに影響されず、バランス調整で見るべき場所も `BattleSimulator` 1クラスに限定できました。特殊能力（先制・狂化・強打・鉄壁・再生・幸運）や成長判定もすべてSimulator内で完結し、ログとして出力されます。

### 状態の一元管理

シーンをまたぐ状態（所持コイン・ラウンド数・対戦カード・賭け情報・試合結果）は `GameSession`（DontDestroyOnLoadのシングルトン）に集約しました。
プロパティは `private set` とし、変更は `TryConsumeCoin` / `AddCoin` / `SetBet` などのメソッド経由に限定しています。コイン消費は成否を `bool` で返し、呼び出し側が失敗時の遷移を判断する形にしました。
前作で通貨を2箇所で別々に持ってしまった反省を、そのまま設計に反映した部分です。

### シーン＝状態のステート管理

ゲームの流れを Title → Bet → Battle → Result →（3回繰り返し）→ AllResult とシーン単位で分割し、各シーンのControllerが自分のシーンだけに責任を持つ形にしました。
各Controllerの冒頭では前提条件（セッションの存在・賭け情報の有無・コイン残高）を検証し、満たさない場合はTitleやBetへ戻すようにしています。デバッグ中にシーンを単体で再生しても壊れないための対策です。

### フォルダ構成

```
Assets/Script/
├── Title / Bet / Battle / Result / AllResult   … シーンごとのController
└── Common/
    ├── Models/   … MonsterData, BetData, MatchResultData など（Serializableな純C#クラス）
    ├── Enum/     … PredictionSide, SpecialAbilityType など
    ├── Systems/  … BattleSimulator, MonsterFactory（純C#ロジック）
    ├── UI/       … ActionLogView, StatusPanelView（表示専任）
    └── GameSession.cs
```

前作のGit運用の反省から、今回は作業の区切りごとにブランチを分け、進捗を後から追える形にしました。

## 生成AIの活用について

本作では、生成AIをステート管理の考え方の整理、UniTaskの書き方の確認、実装方針の相談、エラー原因の切り分け、コード改善案の比較に活用しました。
出力をそのまま使うのではなく、仕様に合っているかをUnity上で動作確認しながら修正しています。

## コードを読み返して見つけた問題

### 「入れたかった機能」の残骸が空ファイルとして残っている

`BetCalculator.cs`（中身はテンプレートのまま）、`InspectCostTable.cs`、`CoinView.cs`、`RoundData.cs`、`BattleOutcome.cs` など、作る予定だったが実装しなかった空スクリプトが複数残っています。
オッズ機能・調査コスト機能を構想していた痕跡ですが、使わないと決めた設計は消すべきでした。迷いの履歴はブランチに残せばよく、mainに残す必要はなかったです。

### コアになり得た仕様を演出に後退させた

`InspectLevel`（None / Basic / Full）というenumと、段階的に情報を開示する `StatusPanelView` を用意しながら、実際には常にFull固定です。
「賭ける前に、コインを払ってどこまで相手の情報を見るか」という駆け引きが本来のコアになり得ましたが、実装しきれず情報開示は無料の演出になりました。1週間という制約の中で、機能を削るなら仕様ごと削る判断を先にすべきでした。

### UI開閉の実装が力業

`BetSceneController` の画面切り替えが、テキスト・画像・ボタンを1コンポーネントずつ `enabled` で切り替える列挙になっています。パネル単位で `SetActive` や `CanvasGroup` にまとめれば数行で済み、要素追加時の修正漏れも防げます。`CloseStatusUI()` が中身のないまま残っているのは、その修正漏れの実例です。

### 細かな設計ミス

- `SceneNames` が定数を持つだけなのに `MonoBehaviour` を継承している（static classにすべき）
- 配当倍率（2倍）が `BattleSimulator` 内に直書きされている。本来 `BetCalculator` に切り出す予定だったが、空ファイルだけ残った

## 学んだこと

- ロジックと演出を分離すると、バランス調整・演出調整・デバッグのすべてが楽になる
- 機能の取捨選択は「実装を諦める」ではなく「仕様から削る」で行う。中途半端に残すと設計が濁る
- 「構造を先に決める」ことでロジック側のバグは前作より減ったが、空ファイルや未実装の残骸のように「決めた構造を最後まで綺麗に保つ」部分にはまだ詰めが甘い

## 前後の作品

- [Slot（学園祭展示）](https://github.com/fsheinsya/UnitySlotGame) — 設計を意識する前の出発点
- [毛一つ（mouhitotu）](https://github.com/fsheinsya/mouhitotu) — データ分離への挑戦と、状態管理の失敗
