# Central-Server

This is the central API for managing trains, and its code is configured to only work through the Authentik proxy at [auth.autotf.de](https://auth.autotf.de).

And it also serves as a sync, for trains and tablets to upload logs, videos, and sync allowed MAC addresses.


It's part of the product "Central Bridge" and is neccessary to further configure trains, and configure security keys, etc.


A desktop client for this API is being developed at [AutoTF-Rail/AutoTf.Manager](https://github.com/AutoTF-Rail/AutoTf.Manager.git).


**All info given in this repository are without warranty. AutoTF and individiuals representing AutoTF are not liable for any damanges occuring due to the contents unless specified otherwise in a contract.**


## API

### Available endpoints:

> /token [GET]: Redirects the user to a /tokenStep, along with their authentik session token. Used for manager app.
> /tokenStep [GET]: Redirects the user to a local endpoint at localhost:5000/token to login the manager app.
> /sync
    > /mac
        > /lastmacaddrsupdate [GET]: Gets the last time a update happened on the list of allowed MAC addresses.
        > /macaddress [GET]: Gets all allowed MAC addresses. Should be only used when /lastmacaddrsupdate returns a newer value than the own local one.
        > /addAddress [GET]: Adds a MAC address to the list of allowed addresses.
    > /keys
        > /validate [GET]: Validates a given code against the timestamp and the secret of the key (by hand of the given serial number). Used when adding a new key, to validate that it works. (Keys can't be used unless verified)
        > /lastkeysupdate [GET]: Gets the last time a update happened on the list of allowed keys.
        > /newkeys [GET]: Gets all keys that have been added since the given timestamp.
        > /allkeys [GET]: Gets all allowed keys.
        > /addkey [POST]: Adds a key by the given serial number and secret.
    > /device
        > /getvideo [GET]: Returns a download for a video from a given device and date.
        > /indexvideos [GET]: Returns an index of all videos available for a given device.
        > /uploadvideo [POST]: Endpoint to upload a video. 
        > /lastsynced [GET]: Returns the date that a last device has synced.
        > /status [GET]: Gets the status of a given device (Online/Offline)
        > /uploadlogs [POST]: Endpoint to upload logs.
        > /getlogs [GET]: Gets the logs from a specified device at a specific date.
        > /status [GET]: Returns an index of all logs available for a given device.
        > /devices [GET]: Returns an index of all available/known devices.
        > /updatestatus [POST]: Endpoint to update the status of a device (yourself).


## Contributions

Would you like to contribute to this project, or noticed something wrong?

Feel free to contact us at [opensource@autotf.de](mailto:opensource@autotf.de)