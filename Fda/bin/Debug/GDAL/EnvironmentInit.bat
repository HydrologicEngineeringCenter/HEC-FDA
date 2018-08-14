@echo off
SET PATH=%PATH%;%CD%\bin
SET GDAL_DATA=%CD%\data
SET PROJ_LIB=%CD%\bin\share
SET GDAL_DRIVER_PATH=%CD%\bin\gdalplugins

@echo on
@echo Gdal environment initialized.