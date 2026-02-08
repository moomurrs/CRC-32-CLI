# CRC-32 CLI ![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/moomurrs/CRC-32-CLI/build.yml?branch=master&style=flat&logo=github&label=build%20status) ![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/moomurrs/CRC-32-CLI/test.yml?branch=master&style=flat&logo=github&label=test%20suite)


Compute CRC-32 checksum forwards and backwards

## Background
Files can experience corruption during transport (downloading, file transfer, or even man-in-the-middle attack). To detect the integrity of files, a checksum can be calculated and compared against the known checksum to verify file integrity. IEEE Cyclic Redundancy Check (CRC) is only one type of checksum commonly used. CRC has many variants over the decades. This project implements the [IEEE CRC-32](https://en.wikipedia.org/wiki/Cyclic_redundancy_check) (the divisor is fixed to specification).

Regardless of the divisor, CRC can be calculated forwards (MSB -> LSB) or backwards (LSB -> MSB). Most common application is the "reverse" direction (which is the default for this program).

This project uses C# to implement CRC-32 supporting both text input and file input, calculating either forward and reverse CRC.

A test suite is provided to ensure correct implementation.

See the manual page below to get started.

## Synopsis

CRC-32 <FILE_INPUT> [-][SWITCH] <STRING_INPUT>

## Description

A commandline interface for generating IEEE CRC-32 on a given file or string.

## Switches

### --text or -t

    Compute checksum on a string input.
    Default is file-mode if this switch is NOT specified.

### --forward or -f

    Compute the check using a forward divisor.
    Default is reverse divisor if this switch is NOT specified.

### --decimal or -d

    Output the checksum in decimal format.
    Default is hexidecimal if this switch is NOT specified.

## Examples

### Example 1

`CRC-32 -t "ABC"`

Compute the checksum on string ""ABC"" using the default reverse divisor.
Show the checksum in default hexidecimal.

### Example 2
`CRC-32 file.mp3 -f`

Compute the checksum on file ""file.mp3"" using the forward divisor.
Show the checksum in default hexidecimal.

### Example 3
`CRC-32 -t "ABC" -f -d`

Compute the checksum on string ""ABC"" using the forward divisor.
Show the checksum in decimal.


## References:
- https://fuchsia.googlesource.com/third_party/wuffs/+/HEAD/std/crc32/README.md
- https://emn178.github.io/online-tools/crc/
- https://crccalc.com/
- Test music file provided by [Ross Bugden](https://www.youtube.com/watch?v=BnmglWHoVrk) (for test suite)
