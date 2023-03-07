# SimulatorGAN
Unity Repository for Simulation Environment Generation using GANs.

# Running headless mode
To run headless mode:

`./Headless/simgan.x86_64 -batchmode -nographics`

# API Request

* Currently, there is only a POST request possible.

* Post to **http:localhost:8080/simulate**

* Post a request with the following body:

    `{"matrixString": "1 0 0 0 0 1 1 1 0 0 0 1 0 1 \n 0 1 1 1...."}`

_Preferably, use **32x32** matrices (for now)_

* Response will contain the following:

        {
            "simulationTime": 19.21 (float),

            "collisionCount": 4 (int)
        }