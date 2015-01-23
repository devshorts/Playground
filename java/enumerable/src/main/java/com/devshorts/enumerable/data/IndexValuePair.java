package com.devshorts.enumerable.data;

public class IndexValuePair<T> {
    public final T value;
    public final int index;

    public IndexValuePair(T value, int index){
        this.value = value;
        this.index = index;
    }
}
