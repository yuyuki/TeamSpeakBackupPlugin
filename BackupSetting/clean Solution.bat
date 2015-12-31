rem Launch it for deleting all folders obj and bin
cls
FOR /D %%d IN (bin,obj,_NCrunch_*,_ReSharper*) DO rmdir "%%d" /s /q
