# HearthstoneCardSimulator
This allows you to run the Hearthpwn card pack simulator through automation.

# Running the application
Simply run any of the .bat files (you need to replace {userName} with your www.hearthpwn.com username and {password} with your password.

WebTests/run_tgt.bat or WebTests/run_wild.bat

There are a few options that are described in the .bat files.

As additional simulators are added new .bat files can be created to run those card pack simulators, assuming their site doesn't drastically change.  The default delay is 2500 milliseconds which is the minimum I find before you're not allowed to make additional requests.  Any anti-DDOS on their end.
