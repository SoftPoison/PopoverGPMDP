# PopoverGPMDP

Popover for Google Play Music Desktop player for Windows.

Pull requests welcome.

## Motivation

Windows' track information in the volume overlay thing is awful - it takes too long to disappear and consumes too much of the screen. It also does not display certain other pieces of information like album name, and whether or not shuffle is enabled.

I also wanted a notification of some sort when the play state or song has changed, which Windows could not do. Hence, this project was born.

## Planned features

* Some sort of configuration system that doesn't require recompiling, probably via a JSON file
* Different config options
  * Background/highlight/text colours
    * Could sync with GPMDP for default colours as .settings.json exists
  * Position
  * Popover duration / speed
  * File checking frequency
* An easy GUI based configuration
* A better default/missing album art icon
* A fancy program icon
