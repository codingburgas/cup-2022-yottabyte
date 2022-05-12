# How to install
## Build the WebApp from source
### Ще го оправя след часа по мат -> Тоест 13:00
0. Make sure you have Visual Studio 2019 installed with ASP.NET and web development workload.
1. Clone the repository.
    -   Open your favourite terminal and navigate to the directory where you want to clone the repository.
    -   Type: `git clone https://github.com/codingburgas/cup-2022-yottabyte.git`
2. Open the `cup-2022-yottabyte\webapp` directory.
3. To lunch the VS Solution click on `Yottabyte.sln`.
4. Before running the project please make sure that you have created a `.appsetting` file in the root directory of the solution. Трябва да го довърша
  - `ADMIN_USERNAME` - the username of the admin
  - `ADMIN_EMAIL` - the email of the admin
  - `ADMIN_PASSWORD` - the password of the admin

## Use the release
1. Download the release from [GitHub](https://github.com/SSIvanov19/lathraea-rhodopaea/releases/download/v1.0.0/Release.zip) or from the [download page.](DOWNLOAD.md)
2. Unzip the file.
3. Rename `.env.example` to `.env`
4. Change the environment variables in the `.env` file
    - `ADMIN_USERNAME` - the username of the admin
    - `ADMIN_EMAIL` - the email of the admin
    - `ADMIN_PASSWORD` - the password of the admin
5. To run the application, just double click on the `lathraeaRhodopaea.exe` file.