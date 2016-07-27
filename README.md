# Thread Pool + Task Testing

To support investigation on PR https://github.com/dotnet/coreclr/pull/5943

To run

Install [.NET Core](https://dot.net/)

In the project directory enter:

`dotnet run -c Release`

Output
```
Testing 2,621,440 calls, with GCs after 262,144 calls.
Operations per second on 4 Cores
                                                                       Parallelism
                            Serial          2x         16x         64x        512x
QUWI No Queues            11.423 M    11.115 M    10.067 M    11.820 M    12.622 M
- Depth    2               9.895 M     8.600 M     9.218 M     9.679 M     9.582 M
- Depth   16              10.260 M    10.041 M    10.088 M    10.354 M    10.198 M
- Depth   64               9.915 M    10.334 M    10.484 M    10.340 M    10.299 M
- Depth  512              10.251 M    10.556 M    10.403 M    10.371 M    10.459 M

SubTasks                  78.045 M   153.537 M   253.733 M   247.775 M   229.333 M
- Depth    2               1.171 M     2.168 M     5.976 M     7.038 M     7.372 M
- Depth   16             443.256 k     1.168 M     3.645 M     4.734 M     4.859 M
- Depth   64             489.519 k     1.363 M     3.745 M     4.135 M     4.924 M
- Depth  512             386.861 k     1.131 M     3.264 M     3.869 M     4.599 M

QUWI Local Queues         10.936 M    11.127 M     9.941 M    12.047 M    12.173 M
- Depth    2               9.771 M     8.550 M     9.091 M     9.648 M     9.471 M
- Depth   16               9.981 M    10.148 M    10.229 M    10.247 M    10.153 M
- Depth   64              10.147 M    10.510 M    10.448 M    10.431 M    10.327 M
- Depth  512              10.352 M    10.490 M    10.386 M    10.492 M    10.371 M

Yielding Await           701.116 k     1.643 M     3.979 M     4.061 M     4.102 M
- Depth    2               1.011 M     2.264 M     5.062 M     5.199 M     5.175 M
- Depth   16               2.687 M     5.944 M     7.223 M     7.217 M     7.067 M
- Depth   64               3.456 M     7.115 M     7.362 M     7.242 M     7.389 M
- Depth  512               3.455 M     6.394 M     6.674 M     7.003 M     6.758 M

Async Awaited            637.122 k     1.001 M     2.942 M     3.054 M     3.032 M
- Depth    2             975.013 k     1.356 M     4.108 M     4.442 M     4.347 M
- Depth   16               1.597 M     3.426 M     6.690 M     6.881 M     7.164 M
- Depth   64               1.800 M     3.796 M     7.463 M     7.552 M     7.573 M
- Depth  512               1.956 M     3.899 M     6.222 M     6.445 M     7.233 M

Async PassThrough        622.543 k     1.208 M     2.070 M     2.985 M     2.931 M
- Depth    2               1.230 M     1.734 M     5.843 M     6.051 M     6.303 M
- Depth   16              10.097 M    11.685 M    46.899 M    48.385 M    47.267 M
- Depth   64              34.866 M    26.243 M   177.084 M   183.501 M   156.581 M
- Depth  512             259.731 M   145.671 M   943.371 M   944.799 M   545.860 M

Completed Awaited         23.402 M    46.844 M    86.794 M    87.031 M    84.541 M
- Depth    2              25.867 M    51.571 M    96.864 M    95.968 M    90.979 M
- Depth   16              20.426 M    41.003 M    71.370 M    74.738 M    72.105 M
- Depth   64              19.070 M    37.509 M    69.483 M    69.613 M    68.429 M
- Depth  512              18.505 M    37.003 M    68.398 M    68.070 M    66.456 M

CachedTask Awaited        23.535 M    47.810 M    87.503 M    87.450 M    85.296 M
- Depth    2              25.580 M    53.134 M    97.494 M    97.755 M    72.187 M
- Depth   16              20.300 M    40.570 M    75.277 M    74.743 M    72.691 M
- Depth   64              18.858 M    38.279 M    69.681 M    67.940 M    66.112 M
- Depth  512              18.559 M    37.306 M    66.519 M    67.171 M    64.815 M

CachedTask CheckAwait    109.932 M   217.507 M   412.040 M   376.022 M   346.715 M
- Depth    2             133.830 M   265.645 M   503.484 M   469.145 M   366.062 M
- Depth   16             152.377 M   301.967 M   508.534 M   527.060 M   401.304 M
- Depth   64              87.136 M   172.832 M   311.021 M   318.097 M   270.257 M
- Depth  512              78.085 M   156.138 M   289.030 M   272.524 M   257.097 M

CachedTask PassThrough   129.248 M   250.906 M   482.903 M   458.085 M   388.770 M
- Depth    2             214.354 M   419.887 M   740.876 M   720.770 M   517.192 M
- Depth   16             593.771 M     1.112 B     1.730 B     1.646 B   967.785 M
- Depth   64             951.245 M     1.723 B     2.810 B     2.178 B     1.116 B
- Depth  512               1.446 B     2.590 B     3.938 B     2.861 B     1.148 B
```