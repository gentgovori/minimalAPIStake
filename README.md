# minimalAPIStake

## Table of Contents




- [Usage](#usage)


## Usage

To consume the APIs in this project use the **gambling.http** file included in the project, use **POSTMAN**, or just **cURL**. 

1. **Register an Account**: To start using the application, you need to register an account. Make a POST request to the `/register` endpoint to create an account. Include the required information, such as a email and password.

   ```shell
   curl --location --request POST 'http://localhost:5045/register' \
    --header 'Content-Type: application/json' \
    --data-raw '{
    "email": "playerone@gmail.com",
    "password": "123456789Aa#"
    }'
   
2. **Login**: After registering, you can log in to your account by making a POST request to the `/login` endpoint. Use the same credentials you used during registration. Upon successful login, you will receive a response that contains an authentication bearer token.
   ```shell
   curl --location --request POST 'http://localhost:5045/login' \
    --header 'Content-Type: application/json' \
    --data-raw '{
    "email": "playerone@gmail.com",
    "password": "123456789Aa#"
    }'

3. **Play the game**: To play the game, use the bearer token obtained from the login response and make a POST request to the `/gambleyourlifeaway` endpoint. This is where the main game action happens.
   ```shell
   curl --location --request POST 'http://localhost:5045/GambleYourLifeAway' \
    --header 'Authorization: Bearer  yourbearertoken\
    --header 'Content-Type: application/json' \
    --data-raw '{
    "points": 5,
    "number": 4
    }'

Good luck! :)
