
# Endnu Bedre Vejr 🌦️  
**("Even Better Weather")**

A fun little weekend project recreating the **old DMI (Danish Meteorological Institute)** weather graphics — as close to the original as possible!  
It’s hooked up to DMI’s **Open Data API**, served via a lightweight backend proxy, and rendered beautifully on a browser canvas.  

🔗 **Live demo:** [https://www.endnubedrevejr.dk/](https://www.endnubedrevejr.dk/)

<img width="686" height="484" alt="image" src="https://github.com/user-attachments/assets/cb5c408b-262c-4d3b-b99b-4619a7c99699" />

---

## 🧠 What Is This?

When DMI changed their website years ago, *bedrevejr.dk* (“better weather”) kept the old style alive for a while. 
Eventually DMI decided to retire the old graphics generator that were still running in the background, killing bedrevejr.dk for good.
This project — *endnubedrevejr.dk* (“even better weather”) — takes that spirit and revives it again, purely for fun and nostalgia.

It’s not meant to be a production service — just a geeky tribute to retro Danish weather visuals ☁️🌡️. There's dozens of us!

---

## 🏗️ How It Works

- **Backend:**  
  A simple C# (AspNetCore) proxy for DMI’s Open Data API.  
  It handles requests to avoid leaking the API key, CORS issues and it fetches forecasts directly from DMI.

- **Frontend:**  
  A client-side renderer that draws the weather graphics directly on an HTML `<canvas>`, closely mimicking the old DMI visual style.

- **Features:**
  - City search 🔍  
  - Use your browser’s GPS for local weather 📍  
  - Dynamic forecast rendering in that nostalgic DMI look 🎨  

---

## ⚠️ Disclaimer

This project isn’t optimized for speed — **DMI’s API can take 10+ seconds** to respond.  
I haven’t added caching or scheduling  since the goal was mainly to:
- Have some fun
- Relive the classic DMI weather visuals
- Pay tribute to *bedrevejr.dk* and *dmi.dk*

So yeah, just imagine it's that full 56k retro experience.

---

## 🚀 Getting Started

If you want to run it locally:

```bash
dotnet build
dotnet run
````

Then open [http://localhost:3000](http://localhost:3000) (or whatever port is configured).

There's also a docker image
---

## 🧩 Tech Stack

* **Frontend:** HTML5 Canvas, JavaScript
* **Backend:** C# (AspNetCore)
* **Data Source:** [DMI Open Data API](https://confluence.govcloud.dk/display/FDAPI/DMI+Open+Data)

---

## 💬 Final Thoughts

This was a quick, nostalgic weekend build — a “meta joke” and a trip down memory lane.
Feel free to fork, remix, or just enjoy some good old-fashioned *endnu bedre vejr* 🌦️

---

**Made with ❤️, JavaScript, and weather nostalgia**
by [@maesse](https://github.com/maesse)
