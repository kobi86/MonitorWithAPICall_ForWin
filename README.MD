(https://miro.medium.com/max/1400/1*zc1BKfAHkpvrZlHPbUvuYA.png)
This is a Windows Service that Monitors A folder and triggers an API CALL to VSTS and triggers a build. 
This App is in C# and containes an AppSettings.json file which helps in managing the settings.

- Folder1 =  Folder 1 To monitor
- Folder2 = Folder 2 to monitor
- Folder3 = Folder 3 to monitor
- Filesloglocation = The Location for the created log file , all the <br />
files that the service monitored will be written into this Log
- AccessToken = Token for the VSTS (PAT)
- BuildID = The call for the API, No need to change it , just update the 
ID of the build and rebuild . <br /> EXAMPLE:"{ \"definition\": {\"id\": 1234}}"
- ApiCallTime = The time between logging the file and until you call the 
API (MilliSeconds)
- OrgName - The name of the VSTS organization
- ProjectName - The name of the project in the VSTS organization

*NOTE: The service all monitor for **created files** and not deleted,renamed etc.. <br />
*NOTE: The Servcie can monitor up to 3 folders ,Currently its monitoring 1 

#Installation: 
- Build the project
- Naviaget to the output folder and search for the EXE file
- Open CMD with as Admin and type : sc.exe create SERVICE_NAME binpath= "SERVICE_FULL_PATH" ,Exmaple: sc.exe create monitorservice binpath= "c:\temp\Project\monitor.exe" 
  Or use the Installationscripts folder inside the repo.
