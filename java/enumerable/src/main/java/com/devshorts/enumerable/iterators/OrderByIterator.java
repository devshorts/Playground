package com.devshorts.enumerable.iterators;

import com.devshorts.enumerable.Enumerable;
import com.devshorts.enumerable.data.ProjectionPair;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.function.Function;

public class OrderByIterator<TSource, TProjection> extends EnumerableIterator<TSource> {


    private List<ProjectionPair> buffer;
    private Function<TSource, Comparable<TProjection>> projection;
    private Comparator<TProjection> comparator;
    private Integer idx = 0;

    public OrderByIterator(Iterable<TSource> source,
                           Function<TSource, Comparable<TProjection>> projection,
                           Comparator<TProjection> comparator) {
        super(source);

        this.projection = projection;
        this.comparator = comparator;

        sort();
    }

    @Override
    public boolean hasNext(){
        return idx < buffer.size();
    }

    @Override
    public TSource next(){
        TSource value = (TSource)buffer.get(idx).value;
        idx++;
        return value;
    }

    private void sort(){
        idx = 0;

        buffer = Enumerable.init(evaluateEnumerable())
                .map(value -> new ProjectionPair(projection.apply(value), value, new Comparator<ProjectionPair<TProjection, ?>>() {
                    @Override
                    public int compare(ProjectionPair<TProjection, ?> o1, ProjectionPair<TProjection, ?> o2) {
                        return comparator.compare(o1.projection, o2.projection);
                    }
                }))
                .toList();

        Collections.sort(buffer);
    }

    private List<TSource> evaluateEnumerable(){
        List<TSource> r = new ArrayList<>();
        while(super.hasNext()){
            r.add(super.next());
        }
        return r;
    }

}
