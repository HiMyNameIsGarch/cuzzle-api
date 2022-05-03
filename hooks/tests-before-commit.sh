#!/bin/sh

if ! dotnet test; then
    echo "Unit tests failed!"
    exit 1
fi
