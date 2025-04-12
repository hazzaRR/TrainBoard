## Run train board C# app on boot
1) Copy over the output file of the dotnet project into the /opt/TrainBoard/ directory

```
sudo /opt/dotnet/dotnet publish -c Release -o /opt/TrainBoard
```

```
sudo chown <current_user> /opt/TrainBoard/
```

1) create the unit file
```
sudo nano /lib/systemd/system/trainboard.service
```
3) Copy the following into the file:
```
[Unit]
Description=National Rail Live Train board
After=multi-user.target

[Service]
ExecStart=sudo /opt/dotnet/dotnet /opt/TrainBoard/TrainBoard.dll

[Install]
WantedBy=multi-user.target

```

4) Save and exit with _ctrl_ + _x_, followed by _y_ when prompted to save, and then _enter_.

We need to tell systemd to recognize our service, so enter:

```
sudo systemctl daemon-reload
```

Note that you will need to enter this command every time you change your .service file, as systemd needs to know it has been updated.

We then need to tell systemd that we want our service to start on boot, so enter:

```
sudo systemctl enable trainboard.service
```

Reboot with `sudo reboot` to verify that your program works. The LED should begin blinking after the Pi boots!

```
systemctl status trainboard
```
check status of service;
