# Checkers AI with Minimax & Alpha-Beta Pruning

## 🎮 Gameplay

This project implements an **advanced Checkers game in Unity** with a powerful **AI opponent** using **Minimax and Alpha-Beta Pruning**. It offers multiple game modes and difficulty levels.

### 🕹️ Game Modes:
1. **Player vs Player (PvP)** – Two human players take turns.
2. **Player vs AI (PvAI)** – Play against an AI opponent with different difficulty settings:
   - **Easy** – Limited depth, basic moves.
   - **Medium** – Smarter decisions, better board evaluation.
   - **Hard** – Deep search, optimal strategy.
3. **AI vs AI (AIVsAI)** – Watch two AI opponents play against each other.

### 🏆 Rules:
- Players **move diagonally** and must **capture when possible**.
- A piece reaching the last row **becomes a King** and moves **both forward & backward**.
- The **game ends when a player has no more moves** or loses all their pieces.

---

## 🏗️ Code Architecture (Composition over Inheritance)

This project follows **Composition over Inheritance** to maintain a **clean and flexible design**.

### Key Components:
- **GameManager** – Handles players, turns, and game state.
- **BoardHandler** – Manages the board and valid moves.
- **CheckerHandler** – Handles **piece selection, movement, and AI execution**.
- **AIHandler** – Implements **Minimax with Alpha-Beta Pruning** for AI moves.

### ✅ Why Composition?
- **More flexibility** – Each component has a clear responsibility.
- **Better reusability** – AI, board, and game logic are modular.
- **Avoids deep inheritance chains** – Reduces complexity.

---

## 🧠 Minimax Algorithm with Alpha-Beta Pruning

The AI uses **Minimax**, a decision-making algorithm for **turn-based games**, optimized with **Alpha-Beta Pruning** to reduce unnecessary evaluations.

### 🔍 How Minimax Works:
1. **Expands possible moves** recursively, evaluating outcomes.
2. **Maximizing Player (AI)** picks the highest value move.
3. **Minimizing Player (Opponent)** tries to minimize AI’s best move.

### ✂️ Alpha-Beta Pruning:
- **Cuts off** branches **that won't affect the final decision**, speeding up computation.

### 📊 Example Minimax Tree:
![Minimax Tree](minimax.jpg)

Each **leaf node** represents a game state score, propagating **optimal decisions** back to the root.

![AI Playing Checkers](alphabeta.gif)
Each **red node** represents a **pruned branch**, meaning that the AI **did not need to explore** these moves.  
This optimization reduces the number of evaluated nodes, allowing the AI to **make faster and smarter decisions** without affecting the outcome.

### 🧠 How It Works:
1️⃣ **Evaluate leaf nodes** to determine game outcomes.  
2️⃣ **Propagate values upward** using Minimax.  
3️⃣ **Prune branches** where a move is already worse than an explored option.  
4️⃣ **Speed up decision-making** while maintaining accuracy.

---

## 🚀 Features & Future Improvements
- ✅ **Real-time AI statistics** (evaluated moves, search depth).
- ✅ **AI prioritizes captures & kings** for better gameplay.
- 🔜 **Custom AI personalities** (aggressive, defensive, balanced).
- 🔜 **Online multiplayer mode**.

---
## 📜 Credits & License
This project is open-source. Feel free to **contribute, modify, or improve** it! 
