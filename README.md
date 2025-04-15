# Central-Server

This is the central API for managing trains, and its code is configured to only work through the Authentik proxy at [auth.autotf.de](https://auth.autotf.de).

And it also serves as a sync, for trains and tablets to upload logs, videos, and sync allowed MAC addresses.


It's part of the product "Central Bridge" and is neccessary to further configure trains, and configure security keys, etc.


A desktop client for this API is being developed at [AutoTF-Rail/AutoTf.Manager](https://github.com/AutoTF-Rail/AutoTf.Manager.git).


The status of the AutoTF infrastructure that hosts the central server can be seen at [status.autotf.de](https://status.autotf.de).


Although the master branch is the most active working version of the central servers, some servers might not be updated yet.

The ongoing development is being done on the develop branch, and merged when ready.

**All info given in this repository are without warranty. AutoTF and individiuals representing AutoTF are not liable for any damanges occuring due to the contents unless specified otherwise in a contract.**


---


## API

The API can be reached at `https://<evuName>.server.autotf.de`

### Available endpoints:

## Token Endpoints

- **`GET /token`**  
  Redirects the user to `/tokenStep`, along with their Authentik session token. Used for the manager app.

- **`GET /tokenStep`**  
  Redirects the user to a local endpoint at `http://localhost:5000/token` to log in to the manager app.


## Sync Endpoints

### MAC Address Management

- **`GET /sync/mac/lastmacaddrsupdate`**  
  Retrieves the last time an update occurred on the list of allowed MAC addresses.

- **`GET /sync/mac/macaddress`**  
  Retrieves all allowed MAC addresses. Should only be used when `/lastmacaddrsupdate` returns a newer value than the local one.

- **`GET /sync/mac/addAddress`**  
  Adds a MAC address to the list of allowed addresses.

### Key Management

- **`GET /sync/keys/validate`**  
  Validates a given code against the timestamp and the secret of the key (using the provided serial number). Used when adding a new key to ensure it works. *(Keys can't be used unless verified.)*

- **`GET /sync/keys/lastkeysupdate`**  
  Retrieves the last time an update occurred on the list of allowed keys.

- **`GET /sync/keys/newkeys`**  
  Retrieves all keys that have been added since the given timestamp.

- **`GET /sync/keys/allkeys`**  
  Retrieves all allowed keys.

- **`POST /sync/keys/addkey`**  
  Adds a key using the given serial number and secret.

### Device Management

- **`GET /sync/device/lastsynced`**  
  Retrieves the last sync date for a device.

- **`GET /sync/device/status`**  
  Gets the status of a specified device (Online/Offline).

- **`GET /sync/device/devices [Obsolete] `**   
  Returns an index of all available/known devices. Please use `/sync/device/getAllTrains`

- **`POST /sync/device/addTrain`**   
  Adds a new train by the given name, auth name, and train id.

- **`POST /sync/device/editTrain`**  
  Edits a train by the given id to change its name, auth username, and train id.

- **`POST /sync/device/deleteTrain`**  
  Deletes a train by the given id.

- **`POST /sync/device/getAllTrains`**  
  Returns a list of all available trains.

- **`POST /sync/device/updatestatus`**  
  Updates the status of a device (itself).

  ### Video Management

  - **`GET /sync/device/video/index`**  
    Returns an index of all available videos for a specified device.

  - **`GET /sync/device/video/download`**  
    Returns a downloadable video from a specified device and date.

  - **`POST /sync/device/video/upload`**  
    Uploads a video.
  
  ### Logs Management

  - **`GET /sync/device/logs/index`**  
    Returns an index of all available logs for a specified device.

  - **`GET /sync/device/logs/download`**  
    Retrieves logs from a specified device for a specific date.

  - **`POST /sync/device/logs/upload`**  
    Uploads logs.

## Info & Contributions

Further documentation can be seen in [AutoTF-Rail/AutoTf-Documentation](https://github.com/AutoTF-Rail/AutoTf-Documentation)


Would you like to contribute to this project, or noticed something wrong?

Feel free to contact us at [opensource@autotf.de](mailto:opensource@autotf.de)
