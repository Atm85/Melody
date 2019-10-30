#!/bin/bash
if [ ! -e "Melody.tar.gz" ]
then
   echo "File not found!"
   tar -cvzf Melody.tar.gz *
else
   echo "File has been found!"
fi
