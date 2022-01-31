# Server Transfer bot

### simple file transfer between server and local computer using Telegram bot.

There is application like FileZilla that use ftp to do same thing. but telegram bot with some internets are faster like my own internet.

I plan to add file exploring, zip/unzip files and i don't know it's possible or not but i want add some function that you can run executables in server, to test program like telegram bots that mostly are single file.



- [ ] Uploading File 
- [x] Downloading File
- [ ] File Exploring
- [ ] zip file in server side then send it.
- [ ] Execute Programs in server
- [ ] Make uploaded app start in boot
- [ ] Replace file of uploaded project(app) with new version



------

### How to use it : 

1. Create your bot with 'bot father' bot in telegram 
2. Get token from bot father
3. Download ServerTransfer_bot program in your server
4. Run it.
5. Give token to program.
6. Start use bot from telegram client.

------

### How To Download in Server :

1. Download Release file with wget :

   ```bash
   sudo wget https://github.com/Nima-abdoli/ServerTransfer_bot/releases/download/v0.1.5/ServerTransfer_bot.v0.1.5.zip
   ```

   

2. Unzip the downloaded file 

   ```bash
   sudo unzip ServerTransfer_bot.v0.1.5.zip
   ```

   

3. Make "ServerTransfer_bot" file executable(forgot to do it before release it.)

   ```bash
   sudo chmod +x ServerTransfer_bot
   ```

    

   > Make Sure to do all of this in your Intended directory
