## modular Compute Shader based Particle System 

After understanding deferred lightning and unity's render pipeline with its command buffers, I wanted to find a clean way to create modular particle system. 

Basically, there are different compute shaders which all render to the same buffers.
Each Compute Shader is controlled by a script whereas its base is inherited from a ParticleBase class which again are managed by one ParticleCSManager.

![gif](https://imgur.com/T3gB5J3.gif)

feel free to use this code as you wish, any contributions and pull requests are welcome.
Tested on a regular laptop with Win10 using unity 2019.1.10f1, spawning 1.5mio particles.

## thanks
Obviously I learned a lot creating [this](https://github.com/aivenhoe/simple-deferred-gpu-cubes "this") little project, so kudos again to the people mentioned here.
The modularized system and even some bits of code is heavily inspired by a workshop held by [milo](https://vvvv.org/users/milo "milo") and [raul](https://vvvv.org/users/Raul "raul"), back in 2015 at the [node15](https://nodeforum.org/activities/festival/node15/ "node15") festival.
