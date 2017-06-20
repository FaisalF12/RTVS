#!/usr/bin/env bash

usage(){
    cat << EOF
 Usage: $0 [options]
  OPTIONS:
     -h         Print this message.
     -m file    MIT License file path.
     -g file    GPLV2 License file path.
     -i file    Input M4 file path.
EOF
}

OPTIND=1

while getopts "h?m:g:i:o" opt; do
    case "$opt" in
    h|\?)
        usage
        exit 0
        ;;
    m)
        MIT_LIC_FILE=$OPTARG
        ;;
    g)
        GPL_LIC_FILE=$OPTARG
        ;;
    i)
        IN_FILE=$OPTARG
        ;;
    esac
done

shift $((OPTIND-1))
[ "$1" = "--" ] && shift

(cat "$MIT_LIC_FILE" | awk '/^/ {print " " $0};' > MIT.lic.out) && \
(cat "$GPL_LIC_FILE" | awk '/^/ {print " " $0};' > GPL.lic.out) && \
m4 --define=MIT_FILE="MIT.lic.out" --define=GPL_FILE="GPL.lic.out" "$IN_FILE"