# Web Archiver
This dotnet 8 application retrieves and archives a copy of a given webpage.
This can be deployed in an Azure App Service.
The URL to be archived is sent by a staticly served HTML webpage

## What gets stored?
- HTML of the webpage
- CSS Stylesheets

No images or JS files are stored or saved.
This does lead to some issue where relative path images are not served.


## Appsettings.json
```json
{
  "ConnectionStrings": {
    "pages": "Data Source=pages.db" 
  },
  "host": "<domain the app is deployed to>"
}
```
