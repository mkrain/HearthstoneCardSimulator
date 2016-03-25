.\bin\Release\WebTests.exe --url=http://www.hearthpwn.com/packs/simulator/2-hearthstone-tgt -r -m --min=56 -e --max=100000 --delay=2250 --user={userName} --password={password} --cardSet=Tgt

::-r write csv file
::-b leader board enabled
::-e minimum search enabled
::-m maximum search enabled
::--log=logfilename (will be appended with .csv)
::--loginUrl (url to login to the hearthpwn website)
::--password login password
::--user login username
::--url card simulator url
::--delay time in milliseconds before next card run
::--min minimum value before the console waits for input
::--max maximum value before the console waits for input
::--export export the dabase to csv on exit
::--exportFileName name of the csv file exported
::--cardSet card set to tag each card with values: Tgt, Wild, None (Tgt is the default)