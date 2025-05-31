# AutoAcceptLoL

A lightweight WPF desktop application that automatically accepts League of Legends match queues by detecting the "Accept" button on the screen using image recognition (template matching via Emgu.CV).

## ⚙️ Features

- Automatically detects the match-found popup
- Simulates a mouse click to accept the game
- Works silently in the background
- Image-based detection (no memory or client injection)

## 🧰 Built With

- [.NET 8 (WPF)](https://dotnet.microsoft.com/)
- [Emgu.CV](https://www.emgu.com/wiki/index.php/Main_Page) – .NET wrapper for OpenCV
- [InputSimulatorEx](https://github.com/micjahn/InputSimulator) – simulates mouse clicks

## 🖼️ How It Works

1. The app captures your screen every second.
2. It looks for the **"Accept"** button using image matching.
3. If found, it moves the mouse and clicks on it.



## 📁 Setup

1. Clone the repo:
   ```bash
   git clone https://github.com/NOUUUFEEL/AutoAcceptLoL.git
