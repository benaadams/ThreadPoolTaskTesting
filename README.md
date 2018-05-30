# Thread Pool + Task Testing

**Note:** This is for before + after testing, rather than a rigorous algorithm choice comparision.

Supporting investigation on PR [https://github.com/dotnet/coreclr/pull/5943](https://github.com/dotnet/coreclr/pull/5943#issuecomment-235444173)

To run

Install [.NET Core](https://dot.net/)

In the project directory enter:

`dotnet run -c Release`

Output
```
Testing 2,621,440 calls, with GCs after 262,144 calls.
Operations per second on 8 Cores
                                                                             Parallelism
                                  Serial          2x         16x         64x        512x
QUWI No Queues (TP)             10.044 M    10.747 M     8.041 M     7.749 M     6.917 M
- Depth    2                     7.932 M     6.415 M     7.944 M     6.996 M     7.238 M
- Depth   16                     6.402 M     6.878 M     8.355 M     7.031 M     7.249 M
- Depth   64                     8.422 M     8.478 M     8.231 M     7.695 M     8.863 M
- Depth  512                     8.437 M     8.584 M     8.586 M     8.368 M     8.141 M


QUWI Queue Local (TP)            4.901 M     9.400 M    23.801 M    26.040 M    26.068 M
- Depth    2                     7.601 M    13.855 M    24.821 M    29.328 M    29.252 M
- Depth   16                    23.078 M    29.484 M    31.611 M    26.945 M    33.981 M
- Depth   64                    31.642 M    32.983 M    34.954 M    35.366 M    34.126 M
- Depth  512                    32.521 M    34.791 M    33.953 M    34.672 M    33.990 M


SubTask Chain Return (TP)      898.137 k     1.739 M     7.248 M     7.167 M     7.136 M
- Depth    2                     1.003 M     1.772 M     7.487 M     7.403 M     7.462 M
- Depth   16                     1.109 M     1.960 M     7.646 M     7.938 M     8.085 M
- Depth   64                     1.095 M     2.002 M     8.080 M     8.043 M     8.056 M
- Depth  512                     1.164 M     2.154 M     8.089 M     7.947 M     8.071 M


SubTask Chain Awaited (TP)     847.199 k     1.288 M     5.059 M     4.989 M     4.683 M
- Depth    2                   808.195 k     1.370 M     4.759 M     5.106 M     5.206 M
- Depth   16                   833.723 k     1.471 M     5.253 M     5.417 M     5.418 M
- Depth   64                   777.359 k     1.479 M     5.149 M     5.199 M     5.387 M
- Depth  512                   895.397 k     1.683 M     3.935 M     3.853 M     4.140 M


SubTask Fanout Awaited (TP)    453.457 k   661.399 k     2.064 M     2.270 M     2.329 M
- Depth    2                   492.377 k   989.180 k     2.581 M     2.697 M     2.748 M
- Depth   16                     1.202 M     2.612 M     3.728 M     3.640 M     3.768 M
- Depth   64                     1.799 M     3.054 M     4.005 M     3.980 M     3.861 M
- Depth  512                     1.989 M     3.206 M     4.071 M     3.926 M     3.792 M


Continuation Chain (TP)        349.644 k   667.531 k     2.584 M     2.561 M     2.537 M
- Depth    2                   555.369 k     1.090 M     4.169 M     4.288 M     4.310 M
- Depth   16                     1.360 M     2.719 M    10.325 M    10.147 M    10.232 M
- Depth   64                     1.593 M     3.262 M    11.929 M    11.861 M    11.887 M
- Depth  512                     1.758 M     3.624 M    12.288 M    12.471 M    11.936 M


Continuation Fanout (TP)       278.070 k   519.198 k     1.819 M     1.966 M     1.805 M
- Depth    2                   450.286 k   844.646 k     2.769 M     2.903 M     2.932 M
- Depth   16                     1.753 M     3.676 M     6.099 M     6.207 M     6.256 M
- Depth   64                     2.383 M     4.913 M     7.134 M     7.207 M     7.049 M
- Depth  512                     2.451 M     4.909 M     7.508 M     7.270 M     7.446 M


Yield Chain Awaited (TP)         1.044 M     1.979 M     6.621 M     7.089 M     7.153 M
- Depth    2                     1.264 M     2.512 M     7.370 M     7.493 M     7.395 M
- Depth   16                     2.258 M     4.525 M     7.857 M     7.931 M     7.759 M
- Depth   64                     2.601 M     5.412 M     5.931 M     6.609 M     6.956 M
- Depth  512                     2.438 M     5.521 M     6.217 M     4.953 M     3.964 M


Async Chain Awaited (TP)       877.108 k     1.726 M     6.305 M     6.641 M     6.595 M
- Depth    2                     1.752 M     2.534 M     8.045 M     7.873 M     9.123 M
- Depth   16                     2.856 M     5.102 M    13.634 M    13.479 M    13.919 M
- Depth   64                     3.724 M     5.885 M    14.783 M    14.515 M    14.659 M
- Depth  512                     3.298 M     6.895 M    13.799 M    13.538 M    14.904 M


Async Chain Return (TP)        877.807 k     1.721 M     6.291 M     6.487 M     6.567 M
- Depth    2                     1.748 M     3.450 M    12.954 M    13.350 M    13.304 M
- Depth   16                    13.611 M    27.740 M   104.200 M   106.827 M   102.415 M
- Depth   64                    59.884 M   106.529 M   381.800 M   379.117 M   302.173 M
- Depth  512                   532.121 M   638.830 M     1.548 B     1.645 B     1.017 B


Sync Chain Awaited (TP)         44.597 M    80.600 M   181.504 M   182.436 M   177.571 M
- Depth    2                    44.589 M    94.483 M   189.936 M   192.908 M   191.998 M
- Depth   16                    29.307 M    57.941 M   135.404 M   136.525 M   134.216 M
- Depth   64                    26.088 M    52.993 M   129.206 M   126.590 M   128.573 M
- Depth  512                    23.114 M    45.141 M   114.419 M   116.511 M   116.237 M


CachedTask Chain Await (TP)     36.358 M    77.051 M   162.797 M   165.454 M   164.906 M
- Depth    2                    44.399 M    91.067 M   179.136 M   178.372 M   180.290 M
- Depth   16                    29.519 M    57.845 M   132.100 M   138.738 M   138.645 M
- Depth   64                    26.249 M    52.616 M   125.385 M   123.901 M   124.273 M
- Depth  512                    22.398 M    45.116 M   112.513 M   115.769 M   114.120 M


CachedTask Chain Check (TP)    178.499 M   227.607 M   730.369 M   828.364 M   650.352 M
- Depth    2                   194.934 M   397.001 M   720.433 M   800.782 M   683.200 M
- Depth   16                   248.247 M   480.161 M   732.123 M   763.245 M   709.936 M
- Depth   64                   108.622 M   208.493 M   461.148 M   483.322 M   458.021 M
- Depth  512                    93.508 M   184.027 M   425.856 M   454.716 M   417.986 M


CachedTask Chain Return (TP)   185.214 M   178.209 M   849.958 M   775.184 M   830.937 M
- Depth    2                   383.493 M   553.070 M   935.494 M     1.295 B   966.750 M
- Depth   16                     1.530 B     1.299 B     2.182 B     2.241 B     1.278 B
- Depth   64                     1.519 B     2.444 B     2.571 B     2.236 B     1.365 B
- Depth  512                     2.292 B     3.468 B     3.487 B     2.859 B     1.405 B
```
