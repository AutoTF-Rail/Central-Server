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

The API can be reached at `https://<evuName>.server.autotf.de` and is documented at [docs.autotf.de/centralserver](https://docs.autotf.de/centralserver)


## Info & Contributions

Further documentation can be seen in [AutoTF-Rail/AutoTf-Documentation](https://github.com/AutoTF-Rail/AutoTf-Documentation)


Would you like to contribute to this project, or noticed something wrong?

Feel free to contact us at [opensource@autotf.de](mailto:opensource@autotf.de)
