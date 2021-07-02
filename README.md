
<img src="https://github.com/LittleCoinCoin/Plant_Counting/blob/Pre-Release/Documentation/Images/Logos/logoAPT.jpg"/> <img src="https://github.com/LittleCoinCoin/Plant_Counting/blob/Pre-Release/Documentation/Images/Logos/ekinocs-logo.png" width="200"/> <img src="https://github.com/LittleCoinCoin/Plant_Counting/blob/Pre-Release/Documentation/Images/Logos/LOGO_TerresInnovia.jpg" width="200"/>

# Procedural Generation of Crop Fields in Unity HDRP
The goal of this generator is to produce synthetic data of aerial images of crop fields as they would be captured by an Unmanned Aerial Vehicle (UAV).

# Tools
This project uses the game engine [Unity](https://unity.com/) with its High Definition Render Pipeline (HDRP).
It also uses parts of the functionalities of [Unity perception package](https://github.com/Unity-Technologies/com.unity.perception).

# Basic set up
## Install Unity
- Download [Unity Hub](https://store.unity.com/download-nuo?currency=USD) (this is the launcher to manage every versions of Unity and your projects)
- From Unity Hub, install Unity version 2019.4.15f1 or newer. The project is using version 2019.4.15f1 but should also be compatible with any later version of Unity (just accept when prompted with project upgrade on its first launch).
## Open the project
- Clone the repo (do not download a zip file as it is known to have some issues with Unity projects).
- Open the project from Unity Hub by clicking on the "*Add*" button and navigating to the directory of the project.
- Once the Unity Editor is open, make sure the scene "*SampleScene*" is open (i.e. visible in the hierarchy window of the editor). If not, navigate in the project window to  __Assets > Scenes > SampleScene__ and double click on "*SampleScene*".
## Set up the crop field parameters
- In the hierarchy window of the editor, select the "*FieldGenerator*" object.
- In the inspector window of the editor, modify the parameters you want in the "*Field*" component to alter the structure of the virtual crop fields.
- Click on the "*Generate Field*" button at the bottom of the inspector to instantiate a crop field corresponding to the parameters in the field.
- If the parameter "*Monitoring iterations*" of the group "*Field Parameters*" is greater than one; then you can show the corresponding growth stage of the field by setting the relevant index in the input field "*Target Growth Stage*".
## Set up the generation parameters
- In the hierarchy window of the editor, select the "*ScenarioHolder*" object.
- In the inspector window of the editor, in the component "*Custom Scenario*", set the number of fields you want to generate.
- In the inspector window of the editor, in the component "*Meta Image Capture*", decide whether you want to capture the images of the entire crop field or only a subset of it. The grid to produce the subset is computed based on what the camera sees of the crop field (ie. using the drone flight height and the camera's optical properties).
## Generate Data
- Click on the "*Play*" button at the top of the editor to enter Play mode.
- Push keyboard's "*i*" key to start the image capture.
- Once the generation is finished, the application will automatically quit Play mode.
## Access Data
- In the hierarchy window of the editor, select the "*Camera*" (a child of "*Drone*").
- In the inspector window of the editor, in the component "*Perception Camera*", click on the button "*Show Folder*".

# References
 * This generator was used in the following publication:
 >Jacopin, E.; Berda, N.; Courteille, L.; Grison, W.; Mathieu, L.; Cornuéjols, A. and Martin, C. (2021). Using Agents and Unsupervised Learning for Counting Objects in Images with Spatial Organization. In Proceedings of the 13th International Conference on Agents and Artificial Intelligence - Volume 2: ICAART, ISBN 978-989-758-484-8 ISSN 2184-433X, pages 688-697. DOI: [10.5220/0010228706880697](https://www.scitepress.org/PublicationsDetail.aspx?ID=oMt1HgWONLQ=&t=1)
 * There is also a video for a more concise view than the paper:
 
 [![Alt text](https://img.youtube.com/vi/85YllTyfxaQ/0.jpg)](https://www.youtube.com/watch?v=85YllTyfxaQ)
