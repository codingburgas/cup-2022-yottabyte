# Troubleshooting
## Error
### Environment variable not found!
### Ще го оправя след часа по мат -> Тоест 13:00
This can be caused by not having the environment variables file set. To fix it, please follow the steps below:
1. If you are using Visual Studio, you can create a new file in the root directory of the solution.
1. If you are using the release, you can create a new file in the same folder as the executable.
2. Name the file `.env`
3. Place the following variables in the file:
    - `ADMIN_USERNAME` - the username of the admin
    - `ADMIN_EMAIL` - the email of the admin
    - `ADMIN_PASSWORD` - the password of the admin