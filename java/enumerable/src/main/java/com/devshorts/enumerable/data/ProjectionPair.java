package com.devshorts.enumerable.data;

import java.util.Comparator;

public class ProjectionPair<T, Y> implements Comparable<ProjectionPair<T, Y>>{
    public T projection;
    public Y value;
    private Comparator<ProjectionPair<T,Y>> comparator;

    public ProjectionPair(T projection, Y value, Comparator<ProjectionPair<T,Y>> comparator){
        this.projection = projection;
        this.value = value;
        this.comparator = comparator;
    }

    @Override
    public int compareTo(ProjectionPair<T, Y> o) {
        return comparator.compare(this, o);
    }
}
