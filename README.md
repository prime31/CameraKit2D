CameraKit2D
==========

CameraKit2D is still under heavy development. There will be more base behaviors and effectors added as they are created!

CameraKit2D is a 2D camera framework for Unity. This was originally going to be an implementation of the fantastic vocubulary that Itay Karen introduced in his GDC talk (more info available [here](http://bit.ly/1c8bRpI)). After a couple weeks of development the realization that you can't make a great camera system entirely generically was found. Games and cameras are just too specific to have a drop-in, perfect solution. Rather than settle for just a *good* solution CameraKit2D was made into a base framework that is very easy to add your own behaviors to.

Several components are provided out of the box so you can get up and running quickly and with good examples to make your own behaviors and effectors from. Three different smoothing methods are also provided (smooth damp, spring and lerp).


### Base Camera Behaviors

Base camera behaviors are the main controllers of your camera. Included with CameraKit2D in the initial release are the following: Camera Window (vertical, horizontal or both), Dual Forward Focus (direction, threshold or velocity based) and Position Locking (vertical, horizontal or both). They all include Gizmos in the editor so you can visualize how they work. Use these classes as a base to make your own behviors. The only requirement is that you implement the *ICameraBaseBehavior* interface.

You can add and remove base camera behaviors at runtime via the *addCameraBaseBehavior* and *removeCameraBaseBehavior* methods. There can be more than one base behavior affecting a camera at any time. This lets you have separate behaviors for horizontal and vertical camera positioning for example.

Here are some examples of the included base behaviors:
![camera behaviors](http://cl.ly/bf0D/CameraKit2D.png)


### Effectors

Effectors let you modify the base camera behavior. These allow you to augment your camera most often based on a specific region in your level. There can be more than one effector active at any time. Effectors also provide a weight which is used to position the camera. Higher weights mean that the effector has more influence on the camera. The *Cue Focus Ring* effector is included as an example. It is a single or dual ring circular trigger that pulls the camera towards it's center when the player enters. An AnimationCurve is used to modify how much the camera is pulled towards the center.

Creating your own effectors only requires implementing the *ICameraEffector* interface. You can then add and remove your effectors at runtime as needed via the *addCameraEffector* and *removeCameraEffector* methods.


### CameraKit2D Flow

![Flow chart](http://cl.ly/beFK/CameraKit2DFlow.png)


License
-----

[Attribution-NonCommercial-ShareAlike 3.0 Unported](http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode) with [simple explanation](http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US) with the attribution clause waived. You are free to use CameraKit2D in any and all games that you make. You cannot sell CameraKit2D directly or as part of a larger game asset.
