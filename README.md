## Install Dotnet 8

Follow these two steps to install the .NET 8 runtime and SDK on your system.

1) Install the .NET 8 SDK

    ```
    sudo curl -sSL https://dot.net/v1/dotnet-install.sh | sudo bash /dev/stdin --version 8.0.406 --install-dir /opt/dotnet
    ```
2) Add .NET to Your System's PATH

    ```
    echo 'export PATH=$PATH:/opt/dotnet' >> /etc/profile
    source /etc/profile
    ```

## Install Mosquitto Mqtt Broker

1) Install the mosquitto broker

    ```
    sudo apt install -y mosquitto
    ```

2) Enable Auto-Start
    
    To ensure Mosquitto automatically starts whenever your Raspberry Pi boots up, enable its systemd service.

    ```
    sudo systemctl enable mosquitto.service
    ```

3) Test the Installation

    Confirm that Mosquitto is installed correctly and check its version by running this command.

    ```
    mosquitto -v
    ```

4) Configure WebSockets Support

    By default, MQTT communicates over port 1883. To allow your web application to connect to the broker, you need to enable the WebSockets protocol on a separate port.

    Create a new configuration file for Mosquitto:

    ```
    sudo nano /etc/mosquitto/conf.d/mosquitto.conf
    ```

    Add the following lines to the file. This configuration allows anonymous connections, sets up the standard MQTT listener on port 1883, and adds a new listener for WebSockets on port 9001, which is required to allow browsers to connect to a mqtt broker.

    ```
    allow_anonymous true

    listener 1883
    protocol mqtt

    listener 9001
    protocol websockets

    ```

## Raspberry pi 4b Matrix Settings

Inside the /TrainBoard/Services/RgbMatrixService/RgbMatrixService.cs file you may need to change this values to ensure less flickering on your pi if you are using a different model

```
    RGBLedMatrixOptions options = new RGBLedMatrixOptions
    {
        Rows = 32,
        Cols = 64,
        Brightness = 50,
        HardwareMapping = "adafruit-hat",
        GpioSlowdown = 5,
    };
```

## Run train board C# app on boot
1) Publish the project and place the output in the /opt/TrainBoard/ directory

    ```
    sudo /opt/dotnet/dotnet publish -c Release -o /opt/TrainBoard
    ```

    - Change the ownership of the directory to your user account so you can modify the files without sudo:

        ```
        sudo chown $USER /opt/TrainBoard/
        ```

    - Ensure the application can write to its configuration file (matrixSettings.json) by setting the correct permissions:
        ```
        sudo chmod 666 /opt/TrainBoard/matrixSettings.json
        ```

2) Create the Systemd Service File

    - Create a new service file named `trainboard.service`:
        ```
        sudo nano /lib/systemd/system/trainboard.service
        ```

    - Copy and paste the following configuration into the file.
        ```
        [Unit]
        Description=National Rail Live Train board
        After=multi-user.target

        [Service]
        WorkingDirectory=/opt/TrainBoard
        Type=idle
        ExecStart=sudo /opt/dotnet/dotnet TrainBoard.dll

        [Install]
        WantedBy=multi-user.target

        ```

3) Enable and Start the Service.

    After creating the service file, you must tell systemd to recognize it and enable it to run on boot.

    - Reload systemd to register the new service file:

        ```
        sudo systemctl daemon-reload
        ```

    - Enable the service to start automatically on system boot:

        ```
        sudo systemctl enable trainboard.service
        ```

    - Start the service on your Raspberry Pi.
        ```
        sudo systemctl start trainboard.service
        ```

    - You can check the status of your service at any time to troubleshoot or verify it's running:

        ```
        systemctl status trainboard
        ```

## Installing Interactive Web app to control the board

I developed two web applications to control the board, but you only need to install one. I recommend using the Vue app as it is more complete, with features like Wi-Fi connectivity and an interactive map. It also performs better on the Raspberry Pi Zero because it runs client-side, unlike the Blazor app which runs server-side on the Pi.


### Running the Vue dashboard app

1) Install Nginx

    First, install the Nginx package on your Raspberry Pi:

    ```
    sudo apt install nginx
    ```

    You can check the service status to confirm it's running:

    ```
    sudo systemctl status nginx
    ```

2) Configure Nginx for the Site

    Next, create a new site configuration file. This file tells Nginx where to find your web app and how to serve it.

    ```
    sudo nano /etc/nginx/sites-available/TrainBoardLink
    ```

    Paste the following configuration into the file. This simple setup listens on port 80 and serves files from the specified root directory.

    ```
    server {
    listen 80;
    server_name localhost;
    root /var/www/TrainBoardLink;
    index index.html index.htm;
    location / {
    try_files $uri $uri/ /index.html;
    }
    }
    ```

3) Enable the Nginx Configuration

    Create a symbolic link to activate your new configuration. This links your site's configuration from the sites-available directory to the sites-enabled directory.

    ```
    sudo ln -s /etc/nginx/sites-available/TrainBoardLink /etc/nginx/sites-enabled/
    ```

4) Test and Restart Nginx
    ```
    sudo nginx -t
    ```
    If the test is successful, restart Nginx for the changes to take effect.

    ```
    sudo systemctl restart nginx
    ```

5) Build and Copy the Vue Project

    You have two options to get the Vue project onto your Raspberry Pi.

    - Option A: Build on your local machine and transfer
    First, build the project on your local computer by running this command in the `TrainBoardLink` directory:

        ```
        npm run build

        ```
        Once built, use scp to copy the entire project directory to the Pi. Be sure to replace <user> with your Raspberry Pi's username.
        ```
        scp -r . pizero.local:/home/<user>/TrainBoardLink
        ```

    - Option B: Build directly on the Raspberry Pi

        If you prefer, you can copy the raw project files to the Pi and then run the build command directly on the device.

6) Copy Files to the Nginx Directory

    Now, copy the built project files to the directory Nginx is configured to serve from. The command you use depends on where you built the project.

    - If you built on your local machine and copied the entire project:

        ```
        sudo cp -r /home/<user>/TrainBoardLink /var/www/TrainBoardLink
        ```

    - If you built directly on the Pi:
    You only need to copy the dist folder, which contains the final built files.

        ```
        sudo cp -r /home/<user>/TrainBoard/TrainBoardLink/dist /var/www/TrainBoardLink
        ```
7) Update Permissions

    Finally, update the file and folder permissions to ensure Nginx can access and serve the files.

    ```
    sudo chmod -R 755 /var/www/TrainBoardLink/
    ```


### Running the Blazor dashboard app

```
sudo /opt/dotnet/dotnet publish -c Release -o /opt/TrainBoardDashboard
```

```
sudo chown $USER /opt/TrainBoardDashboard/
```

1) create the unit file
    ```
    sudo nano /lib/systemd/system/trainboardDashboard.service
    ```
2) Copy the following into the file:
    ```
    [Unit]
    Description=Blazor app for configuring which National Rail service to display on the matric
    After=multi-user.target

    [Service]
    WorkingDirectory=/opt/TrainBoardDashboard
    Type=idle
    ExecStart=sudo /opt/dotnet/dotnet TrainBoardDashboard.dll

    [Install]
    WantedBy=multi-user.target

    ```

## Change Polkit access for Network Manager

Because the C# worker application runs as a systemd daemon service, which means it operates in the background without an active user logged in. By default, NetworkManager restricts many of its functions—like scanning for Wi-Fi networks, adding connections, or creating a hotspot—to active users for security reasons.

To allow your daemon service to use these functions, you need to create a new Polkit rule. Polkit is a component that controls system-wide privileges for applications. The rule you create will grant your non-active service the necessary permissions.

1) Create a new Polkit rule file using the following command:

    ```
    sudo nano /etc/polkit-1/rules.d/90-trainboard-wifi.rules
    ```

2) Add the following code to the file. 
    ```
    polkit.addRule(function(action, subject) {
        if ((action.id == "org.freedesktop.NetworkManager.wifi.scan" ||
            action.id == "org.freedesktop.NetworkManager.settings.modify.system" ||
            action.id == "org.freedesktop.NetworkManager.wifi.share.protected" ||
            action.id == "org.freedesktop.NetworkManager.network-control") &&
            subject.active == false) {
            return polkit.Result.YES;
        }
    });
    ```

## Setting Up the Hotspot
To create the hotspot, you'll use the NetworkManager command-line tool (nmcli). This command creates a hotspot named BRboard with the password train2go! and immediately activates it.

```
sudo nmcli device wifi hotspot ssid BRboard password train2go!

```

### Important Note on Disconnection
Running the above command will disconnect your current SSH session. This happens because the Raspberry Pi activates 

To reconnect to your Raspberry Pi, follow these steps:

1) On your device, connect to the new Wi-Fi network named BRboard using the password train2go!.

2) SSH back into your Raspberry Pi using its hostname. Your device should be able to find it on the new network. Replace <username> and <hostname> with your specific details.
```

ssh <username>@<hostname>.local

```

To get your Raspberry Pi back online and connected to the internet, you'll need to reconnect it to your primary Wi-Fi network. Use the nmcli connection up command for your preconfigured Wi-Fi connection.

```
sudo nmcli connection up preconfigured
```

If you do not have a preconfigured network (e.g., if you did not set one up in Raspberry Pi Imager), you'll need to connect manually. You can find all available connections by running:

```
nmcli connection show

```
Then, connect to the desired network.