-------------------
Welcome to PowerUI!
-------------------


        Guides and more expansive documentation are available online:

                         http://powerui.kulestar.com/

     This guide is provided as a quick how-to if you don't have internet.
Otherwise, it's reccommended to check out the site as it is lots more informative.



Thank you and please keep me posted on what fabulous creations you have in mind!
If you ever need any help, please just post a message on the PowerUI forums or 
help request page (which emails me directly) and I'll be more than happy to help.




-Getting Started-


1. The PowerUI Tag

Once PowerUI is included in your project, you'll need to create a new tag called 'PowerUI'.
To do this, go to Edit -> Project settings -> Tags.

This will open up the tag editor in your inspector. Then, wherever you have an empty
User Layer (e.g. User Layer 8), click on the right and enter "PowerUI", then hit return.



2. Making PowerUI invisible to other cameras

In the scene you would like to add PowerUI to, you'll need to make sure the Main Camera (and any 
others which render to the screen) don't display the PowerUI layer.
To do this, click on your Main Camera game object to see the camera settings in the inspector.
Click the Culling Mask option (where it says 'Everything' by default) and uncheck PowerUI.



3. Creating The UI

Next up, create a new gameobject and attach the script named "Powerui Manager".
In the inspector, you'll see it wants a Text Asset called Html File. Import
a new empty .html file into your project and select it from the PowerUI Managers inspector
under the Html File property.

Then just add some html into your file with a text editor of your choice and hit play!

Note: If you're using an external editor for your html file, you may need to click out of and back into unity to see changes.




-Adding Images-

Images must be inside a folder called Resources. They can be in subfolders too.
All images must be read/write enabled. To do this, click the image to add in your project folder
and take a look at the inspector.

1. Change the image type to Advanced.
2. Change "Non Power of 2" to none.
3. Check the read/write enabled checkbox.
4. Uncheck generate mipmaps.

Then, hit apply. Your texture can now be referenced just by it's path!
e.g. If it's at Resources/image.png, use "image.png". If it's at Resources/subfolder/image.png, use "subfolder/image.png".

Images don't have to come from your project - check out the dynamic textures example for an idea of that. 




- Creating SPA (Animation) files-

Graphic Animations for PowerUI are stored in our custom .spa file format. This format was made as it's very compact and is
designed for efficiency in Unity. To make one, you'll need a set of frame images. They can be just about any format, and can come from any external tool.
For example, 3D applications such as Autodesk Maya can be set to output a set of images, and so can Photoshop (my personal choice).
These images must then be passed to the SPA generator (freely downloadable from the website), which will produce a .spa file.

To display it in Unity, it must be named .spa.bytes. You can setup the SPA generator to stick .bytes on the end to save time too.

Like an image, it must also be inside a folder called Resources. They can be in subfolders too.

Treat them just like an image when you want to use them. Either use image tags - <img src="myAnimation.spa"/>, or use the background-image:url(myAnimation.spa) css property.
From this point, what you do with it is up to your imagination!




-Publishing your game-

Don't forget to remove the examples folder! The PowerUI examples use Resources folders which
will be included in your published game at the expensive of a few KB of size.

