# Unity-GLTF-Exporter
Export from unity to glTF

*Requires version 2022.1 or above



Note: I havent tried on Mac (and i dont think its working there), but I will try to add support on later time.

Important Setup -> 

1-Clone/copy the full project inside Assets Folder

2- Make sure to install "Shader Graph" package required to create Images for the exporter: 

Window -> PackageManager

In Unity Registry section, search for shader grraph, and install it.


Using the Exporter

1- Add script: "Mono_Export To GLTF" to the desired parent node to export

2- Set at least gltf name, model create folder and export location, and press gltf test build

Adding custom "extras"

1- Add script: "Object Node User Extras Mono" to the desired node to add an "extra" attribute

2- Set desired attributes

3- When exporting, your gltf file will contain any extra attribute added in Unity

Support: memelotsqui@gmail.com

