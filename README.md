# Thread Pool + Task Testing

To run

Install [.NET Core](https://dot.net/)

In the project directory enter:

`dotnet run -c Release 10`

Output
```
Testing 2,621,440 calls, with GCs after 262,144 calls.
Operations per second on 4 Cores
                                                                       Parallelism
                            Serial          2x         16x         64x        512x
QUWI                      11.617 M    11.179 M    10.365 M    11.660 M    12.726 M
- Depth    2               9.834 M     8.553 M     8.990 M     9.604 M     9.785 M
- Depth   16              10.393 M    10.175 M    10.266 M    10.271 M    10.134 M
- Depth   64              10.350 M    10.497 M    10.437 M    10.390 M    10.296 M
- Depth  512              10.548 M    10.415 M    10.238 M    10.261 M    10.515 M

Yielding Await           774.085 k     1.887 M     4.345 M     4.408 M     4.533 M
- Depth    2               1.117 M     2.395 M     5.914 M     6.064 M     5.904 M
- Depth   16               3.093 M     7.014 M     8.193 M     8.176 M     7.849 M
- Depth   64               3.879 M     7.674 M     8.467 M     8.172 M     8.471 M
- Depth  512               4.190 M     8.064 M     7.706 M     8.231 M     7.736 M

Async Awaited            622.927 k     1.045 M     2.957 M     3.305 M     3.244 M
- Depth    2               1.008 M     1.379 M     4.543 M     4.796 M     4.807 M
- Depth   16               1.967 M     3.701 M     7.691 M     7.698 M     7.944 M
- Depth   64               1.958 M     4.261 M     8.140 M     8.390 M     8.254 M
- Depth  512               2.184 M     4.305 M     7.053 M     6.651 M     8.225 M

Async PassThrough        507.584 k   988.983 k     2.892 M     3.375 M     3.411 M
- Depth    2               1.217 M     1.920 M     6.513 M     7.012 M     6.831 M
- Depth   16              10.030 M    16.674 M    52.450 M    55.184 M    46.602 M
- Depth   64              39.119 M    59.738 M   153.788 M   209.949 M   167.410 M
- Depth  512             272.063 M   530.495 M     1.056 B     1.080 B   582.659 M

Completed Awaited         23.971 M    47.706 M    89.612 M    85.742 M    87.153 M
- Depth    2              26.121 M    52.854 M    97.807 M    98.197 M    92.440 M
- Depth   16              21.087 M    40.520 M    58.879 M    57.520 M    74.212 M
- Depth   64              19.600 M    38.357 M    71.900 M    71.180 M    61.276 M
- Depth  512              18.668 M    37.566 M    69.615 M    69.648 M    67.155 M

CachedTask Awaited        23.910 M    47.189 M    90.081 M    89.037 M    87.170 M
- Depth    2              26.365 M    53.936 M    99.279 M    95.802 M    92.564 M
- Depth   16              20.317 M    41.292 M    73.340 M    76.207 M    71.134 M
- Depth   64              19.259 M    38.675 M    71.610 M    71.164 M    69.350 M
- Depth  512              18.423 M    37.311 M    63.619 M    68.582 M    67.492 M

CachedTask CheckAwait    113.805 M   224.727 M   408.114 M   370.213 M   354.177 M
- Depth    2             138.073 M   267.584 M   460.104 M   490.227 M   383.039 M
- Depth   16             154.979 M   300.094 M   559.670 M   552.662 M   427.153 M
- Depth   64              89.945 M   175.174 M   329.136 M   330.814 M   284.152 M
- Depth  512              80.883 M   160.597 M   287.637 M   292.974 M   237.746 M

CachedTask PassThrough   120.429 M   232.946 M   409.600 M   426.806 M   365.052 M
- Depth    2             197.300 M   387.672 M   666.186 M   627.424 M   506.656 M
- Depth   16             578.238 M     1.084 B     1.794 B     1.697 B   979.026 M
- Depth   64             947.840 M     1.774 B     2.970 B     2.336 B     1.082 B
- Depth  512               1.455 B     2.645 B     3.951 B     3.323 B     1.344 B
```