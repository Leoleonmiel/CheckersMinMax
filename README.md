# 🏆 Checkers Game - AI with Minimax & Alpha-Beta Pruning

![Checkers Game](https://upload.wikimedia.org/wikipedia/commons/4/42/Draughts.svg)

## 🎮 Gameplay Overview
This project is an advanced **Checkers game** developed in **Unity**, featuring an **AI opponent** powered by the **Minimax algorithm** with **Alpha-Beta Pruning**. The game supports multiple modes and AI difficulty levels.

### 🕹️ Game Modes
- **Player vs Player (PvP):** Two human players take turns playing.
- **Player vs AI (PvAI):** Play against an AI opponent that makes strategic decisions.
- **AI vs AI (AIVsAI):** Two AI instances compete automatically.

### 📜 Game Rules
- Pieces move diagonally on black squares.
- **Mandatory captures** (if a capture is possible, it must be taken).
- A piece **becomes a King** when reaching the opponent’s back row.
- **Kings can move backward** and are stronger.
- **Winning conditions:** Win by eliminating all opponent pieces or blocking their moves.

---

## 🏗️ Code Architecture - Composition Over Inheritance

This project follows **composition and aggregation** rather than inheritance to enhance **modularity** and **scalability**.

### 📌 Key Components
| Component       | Description |
|----------------|-------------|
| **GameManager**  | Controls game state, turn switching, and player management. |
| **BoardHandler** | Manages board state, pieces, and move validation. |
| **CheckerHandler** | Handles piece movements, captures, and animations. |
| **AIHandler** | Implements Minimax & Alpha-Beta Pruning for AI decision-making. |

### ✅ Why Composition Instead of Inheritance?
- **Better separation of concerns** → AI logic is not tied to GameManager.
- **Easier lifecycle management** → Independent components reduce bugs.
- **More flexibility** → Allows changing or extending game logic without breaking dependencies.

---

## 🤖 Minimax Algorithm & Alpha-Beta Pruning

The **Minimax algorithm** is a decision-making technique used in turn-based strategy games.

### 🔍 How Minimax Works:
1. **Move Tree Generation**: AI simulates all possible moves.
2. **Recursive Evaluation**: AI alternates between maximizing its advantage and minimizing opponent’s advantage.
3. **Board Scoring**:
   - More checkers = higher score.
   - Kings are more valuable than normal pieces.
   - Strategic positions are prioritized.
4. **Optimal Move Selection**: AI chooses the move with the **best possible outcome**.

### 🚀 Alpha-Beta Pruning Optimization
Alpha-Beta Pruning **improves Minimax** by reducing unnecessary calculations:
- **Prunes unpromising moves** early, saving computation time.
- **Allows deeper search** within the same processing time.

### 🧠 AI Strategy Enhancements
- **Prioritizes captures over normal moves**.
- **Avoids infinite loops** (no back-and-forth movement).
- **Adjusts strategy based on remaining pieces** (more aggressive in the endgame).

---

## 🛠️ How to Run the Game
1. **Clone the repository**  
   ```bash
   git clone https://github.com/yourusername/checkers-game-ai.git
   cd checkers-game-ai
   ```
2. **Open the project in Unity**.
3. **Choose a game mode** from the main menu.
4. **Move a piece** by clicking it and selecting a valid square.
5. **Watch AI play in PvAI or AIVsAI mode**.

---

## 🚀 Future Enhancements
- Implement **different AI playstyles** (aggressive, defensive, balanced).
- Improve **heuristics** to anticipate multi-step captures.
- Add **online multiplayer mode**.

---

## 💡 Credits
Thanks to everyone who contributed to this project!  
Enjoy the game! 🎉  
