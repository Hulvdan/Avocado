# Avocado

Various studies applied to a platformer game in Unity, C#.

![Avocado Throwing](./Documentation/avocado-throwing-loop.gif)

## Rationale

This project was done for educational and entertainment purposes.

Also, it could help with assessing the quality of my code during interviews.

## What I did

I applied my studies of the following resources:

- [Book. Game Programming Patterns](https://gameprogrammingpatterns.com/) - Bob Nystrom

- [Book. LevelUp!](https://www.amazon.com/Level-Guide-Great-Video-Design/dp/1118877160) - Scott Rogers

    > I am also a firm believer that almost ANYTHING can be made into gameplay

- [Article. Unity — Project Structure Best Practices!](https://sam-16930.medium.com/unity-project-structure-a694792cefed) - Samuel Asher Rivello

- [Video. Math for Game Programmers: Building a Better Jump](https://www.youtube.com/watch?v=hG9SzQxaCm8) - at 2016 GDC talk, by Kyle Pittman

- [Video. Math for Game Programmers: Juicing Your Cameras With Math](https://www.youtube.com/watch?v=tu-Qe66AvtY) - at 2016 GDC talk, by Squirrel Eiserloh

- [Video. How to Make a Good 2D Camera](https://www.youtube.com/watch?v=TdWFzpgnljs) - Game Maker's Toolkit

- Wwise

  - [Documentation. Wwise Fundamentals](https://www.audiokinetic.com/fr/library/edge/?source=WwiseFundamentalApproach&id=wwise_fundamentals)

  - [Video. Wwise Tutorial - Integrating Wwise Into Unity](https://www.youtube.com/watch?v=qEfQtAM1z4o) - Crosby Nelson

  - [Video. Wwise Tutorial - Posting Events With Custom Scripts In Unity](https://www.youtube.com/watch?v=_voQkyAOckI) - Crosby Nelson

  - [Video. Game Audio with Unity and Wwise Part 2: Let there be Sound!](https://www.youtube.com/watch?v=i2gPkgqVHf4) -  Michael G Wagner

  - [Video. Wwise Tutorial - Playing A Sound In Unity](https://www.youtube.com/watch?v=5Ta1PZmVXik) - Crosby Nelson

  - [Video. Wwise-101 - Lesson 4 - Applying Distance Attenuation](https://www.youtube.com/watch?v=F1eMxm4tu8s) - Audiokinetic

## Study results

There you can find examples of using the following:

- [x] **Throwing seed!**
- [x] **Decoupling** input handling from logic
- [x] **State** design pattern for the character controller
  - [x] Movement
    - [x] Idle
    - [x] Running
  - [x] Jumping
  - [x] Falling
  - [x] Throwing seed

![Character Controller States Diagram](./Documentation/state-machine.jpg)

- [x] Good **project layout** that supports project organization. It also makes sure that nothing unnecessary goes into the build (e.g. dependency on Unity's Editor library)
- [x] Consistent code formatting
- [x] A trick with not instantiating projectiles more than once per avocado controller

![Gravity and initial jump's velocity for achieving jump of specific duration and height](./Documentation/jump-formula.jpg)

- [x] :arrow_up: I got formulas of gravity and initial velocity for achieving a jump of specific duration and height
- [x] Movement
  - [x] Max movement speed
  - [x] Acceleration speed
- [x] Jumping
  - [x] Slower horizontal acceleration while mid-air
  - [x] Jump cutoff
  - [x] Duration til reaching the top point of the jump
  - [x] Higher gravity scale while falling
- [ ] Camera
  - [x] Dead zone
  - [x] Non-instant camera following
  - [ ] Lookahead
  - [ ] Pixel perfect
- [x] Assists
  - [x] Coyote time
  - [x] Buffering inputs
  - [x] Max falling velocity
- [ ] Juice
  - [x] Camera Shake on fall. Used **SimplexNoise** to simulate Perlin noise
  - [ ] Particles
    - [ ] While running
    - [ ] On jump
    - [ ] On grounded
  - [ ] SFX using **Wwise**
    - [x] Footsteps
    - [x] On jump
    - [x] On grounded with different volume
    - [ ] Throwing
    - [ ] On seed colliding with terrain
- [ ] Enemies
