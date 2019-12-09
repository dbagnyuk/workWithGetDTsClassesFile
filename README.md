# workWithGetDTsClassesFile
```
Usage:
        workWithGetDTsClassesFile.exe PA [Srv] [/f {Path}] [/c]
                                         [/st {asc|desc}] [/ss {asc|desc}]

Input:
        PA   - Personal Account (must be 12 digits long).

Optional:
        Srv  - Service code which you looking for
               (must not contain special symbols).
        /st  - {asc|desc} sorting for Terminal Devices
               (default "asc").
        /ss  - {asc|desc} sorting for Service codes
               (default "asc").
        /c   - decline write ouput to console.
        /f   - {Path} path to file where result will be save
               (by deafault will be used "c:\out.txt").

        /?   - for this help.

Example:
        workWithGetDTsClassesFile.exe 277300065848 TEST666 /f c:\ /c /st desc /ss desc
```
