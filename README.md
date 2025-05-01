<div align="center">
  <a href="https://www.playbalatro.com/">
    <img src="https://cdn.cloudflare.steamstatic.com/steam/apps/2379780/header.jpg" alt="RLatro"/>
  </a>
</div>

# Rlatro

[![Steam](https://img.shields.io/badge/Steam-Balatro-blue?logo=steam)](https://store.steampowered.com/app/2379780/Balatro/)
[![WiKi](https://img.shields.io/badge/WiKi-Balatro-blue?logo=fandom)](https://balatrogame.fandom.com/)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)

> **RLatro** is a *clean-room, deterministic* re-implementation of the award-winning poker deck-builder **Balatro** written entirely in **C#**.  
> The project is geared toward **reinforcement-learning research**: headless simulation, reproducible seeds, and (soon) lightning-fast Python bindings.

---

## ✨ Why does this exist?

The official Balatro build is Lua + Löve and optimised for players, not machines.  
RLatro aims to:

* simulate thousands of hands per second in headless mode  
* expose the full game state as a plain struct for vectorised RL  
* keep a deterministic RNG pipeline for reproducibility  
* stay 100 % unit-testable with no graphics dependency  

---

## 📦 Repository layout

```
RLatro.sln              – solution entry
│
├─ RLatro.Core/         – pure game logic & state machine
├─ RLatro.Cli/          – entry point - will be added soon
├─ RLatro.Py/           – (TODO)
└─ tests/               – Unit tests and Benchmarks
```

---

## 🚀 Getting started

### Prerequisites

* .NET 8 SDK (`dotnet --version` >= 8.0)  
* Windows (Linux/MacOs not yet tested)
* *Optional* Python ≥ 3.10 (for upcoming bindings)  

### Build & play a dummy hand

Currently no entry point is provided, more detail to come later.

---

## 🧩 Roadmap

| Milestone | Description |
|-----------|-------------|
| **v0.1.0** | Publish deterministic core + CLI demo |
| **v0.2.0** | First Python wheel (`pip install rlatro`) via pythonnet |
| **v0.3.0** | OpenAI Gymnasium-compliant env, vectorised batch API |
| **v1.0.0** | Stable API freeze, baseline PPO & Q-learning notebooks |

---

## 🤝 Contributing

1. Fork & create a feature branch (`feat/implement-xyz`)
2. Open a PR – We love early feedback and architecture propositions.

**Here are a few points where help is needed**

### 🎲 Implement consumables effects
**Description**: Implement the original balatro consumables effects

**Technical Guidance**: Implement `IEffect` and `IUsageCondition` within the `Balatro.Core.CoreObjects.Contracts.Objects.Consumables` namespace. These implement a method that takes the game context and the action target cards and validates/apply the effect.

### 📊 Additional Metrics & Logging
**Description**: Add/Enhance the logging and metrics system to facilitate better analytics for RL training. Add benchmarking and unit tests of hot-path methods.

**Technical Guidance**: Create or update the logging mechanism. Benchmarking and unit tests should be a separate project.

---

## ⚖️ License

MIT. See [`LICENSE`](LICENSE).

> **Disclaimer**  
> Balatro is © LocalThunk & Playstack. This repository is an unofficial fan-made re-implementation for **research and educational purposes only**. No proprietary assets or original source code are distributed here.

---

_Simulate more, shuffle less. 🃏_

