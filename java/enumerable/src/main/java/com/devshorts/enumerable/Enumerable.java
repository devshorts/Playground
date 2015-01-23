package com.devshorts.enumerable;

import com.devshorts.enumerable.data.Action;
import com.devshorts.enumerable.data.Yieldable;
import com.devshorts.enumerable.iterators.*;

import java.util.*;
import java.util.function.*;

class YieldedEnumeration<TSource> implements Iterable<TSource>  {

    private Supplier<Yieldable<TSource>> generator;

    public YieldedEnumeration(Supplier<Yieldable<TSource>> generator) {
        super();

        this.generator = generator;
    }

    @Override
    public Iterator<TSource> iterator() {
        return new YieldedEnumerationIterator<>(generator);
    }
}

public class Enumerable<TSource> implements Iterable<TSource> {

    //private Iterable source;

    private Function<Iterable<TSource>, Iterator<TSource>> iteratorGenerator;

    public static <TSource> Enumerable<TSource> init(Iterable<TSource> source){
        return new Enumerable<>(ignore -> new EnumerableIterator<>(source));
    }

    public static <TSource> Enumerable<TSource> generate(Supplier<Yieldable<TSource>> generator){
        return new Enumerable<>(ignore -> new EnumerableIterator<>(new YieldedEnumeration<>(generator)));
    }

    public static <TSource> Enumerable<TSource> generate(Supplier<Yieldable<TSource>> generator,
                                                         Action onNewIterator){
        return new Enumerable<>(x -> {
            onNewIterator.exec();

            return new EnumerableIterator<>(new YieldedEnumeration<>(generator));
        });
    }

    private <TResult> Enumerable<TResult> enumerableWithIterator(Function<Iterable<TSource>, Iterator<TResult>> generator){
        return new Enumerable<>(ignore -> generator.apply(this));
    }

    protected Enumerable(Function<Iterable<TSource>, Iterator<TSource>> iteratorGenerator) {
        this.iteratorGenerator = iteratorGenerator;
    }

    public <TResult> Enumerable<TResult> map(Function<TSource, TResult> mapFunc){
        return enumerableWithIterator(source ->
                new MapIterator<TSource, TResult>(source, i -> mapFunc.apply(i)));
    }

    public <TResult> Enumerable<TResult> flatMap(Function<TSource, List<TResult>> mapFunc){
        return enumerableWithIterator(source ->
                new FlatMapIterator<TSource, TResult>(source, i -> mapFunc.apply(i)));
    }

    public Enumerable<TSource> filter(Predicate<TSource> filterFunc){
        return enumerableWithIterator(source -> new FilterIterator<>(source, filterFunc));
    }

    public Enumerable<TSource> take(int n){
        return enumerableWithIterator(source -> new TakeIterator<>(source, n));
    }

    public Enumerable<TSource> takeWhile(Predicate<TSource> predicate){
        return enumerableWithIterator(source -> new TakeWhileIterator<>(source, predicate));
    }

    public Enumerable<TSource> skip(int skipNum){
        return enumerableWithIterator(source -> new SkipIterator<>(source, skipNum));
    }

    public Enumerable<TSource> skipWhile(Predicate<TSource> predicate){
        return enumerableWithIterator(source -> new SkipWhileIterator<>(source, predicate));
    }

    public Enumerable<TSource> iter(Consumer<TSource> action){
        return enumerableWithIterator(source ->
                new IndexIterator<>(source, idxPair -> action.accept(idxPair.value)));
    }

    public Enumerable<TSource> iteri(BiConsumer<Integer, TSource> action){
        return enumerableWithIterator(source ->
                new IndexIterator<>(source, idxPair -> action.accept(idxPair.index, idxPair.value)));
    }

    public Enumerable<TSource> distinctUnion(Iterable<TSource> intersectWith){
        return enumerableWithIterator(source -> new DistinctUnion<>(source, intersectWith));
    }

    public Enumerable<TSource> intersect(Iterable<TSource> intersectWith){
        return enumerableWithIterator(source -> new IntersectIterator<>(source, intersectWith));
    }

    public Enumerable<TSource> except(Iterable<TSource> except){
        return enumerableWithIterator(source -> new ExceptIterator<>(source, except));
    }

    public <TProjection> Enumerable<TSource> orderBy(Function<TSource, Comparable<TProjection>> projection){
        return orderBy(projection, (o1, o2) -> o1.compareTo((TProjection) o2));
    }

    public <TProjection> Enumerable<TSource> orderByDesc(Function<TSource, Comparable<TProjection>> projection){
        return orderBy(projection, (o1, o2) -> o2.compareTo((TProjection) o1));
    }

    public <TProjection> Enumerable<TSource> orderBy(Function<TSource, Comparable<TProjection>> projection,
                                                     Comparator<Comparable<TProjection>> comparator){
        return enumerableWithIterator(source -> new OrderByIterator(source, projection, comparator));
    }

    public <TSecond, TProjection> Enumerable<TProjection> zip(Iterable<TSecond> zipWith,
                                                              BiFunction<TSource, TSecond, TProjection> zipper){
        return enumerableWithIterator(source -> new ZipIterator<>(source, zipWith, zipper));
    }

    public TSource first(){
        return unsafeIterEval(new NthIterator<>(this, 1));
    }

    public TSource firstOrDefault(){
        return orDefault(new NthIterator<>(this, 1));
    }

    public TSource nth(int n){
        return unsafeIterEval(new NthIterator<>(this, n));
    }

    public TSource nthOrDefault(int n){
        return orDefault(new NthIterator<>(this, n));
    }

    public TSource last(){
        return unsafeIterEval(new LastIterator<>(this));
    }

    public TSource lastOrDefault(){
        return orDefault(new LastIterator<>(this));
    }

    public <TAcc> TAcc fold(BiFunction<TAcc, TSource, TAcc> accumulator, TAcc seed){
        return evalUnsafeMapIterator(new FoldIterator<>(this, accumulator, seed));
    }


    /**
     * Folds using the first element as the seed
     * @param accumulator
     * @return
     */
    public TSource foldWithFirst(BiFunction<TSource, TSource, TSource> accumulator){
        return unsafeIterEval(new FoldWithDefaultSeedIterator<>(this, accumulator));
    }

    public Boolean any(Predicate<TSource> predicate){
        return evalUnsafeMapIterator(new PredicateIterator<>(
                this,
                predicate,
                (acc, elem) -> acc || elem,
                i -> i,
                () -> true,
                false
        ));
    }

    public Boolean all(Predicate<TSource> predicate){
        return evalUnsafeMapIterator(new PredicateIterator<>(
                this,
                predicate,
                (acc, elem) -> acc && elem,
                i -> !i,
                () -> false,
                true
        ));
    }

    public Enumerable<TSource> distinct(){
        return enumerableWithIterator(source ->
                new DistinctIterator<TSource, TSource>(source, i -> i));
    }

    public <TProjection> Enumerable<TSource> distinctBy(Function<TSource, TProjection> projection){
        return enumerableWithIterator(source ->
                new DistinctIterator<>(source, projection));
    }

    public List<TSource> toList(){
        List<TSource> r = new LinkedList<>();

        for(TSource item : this){
            r.add(item);
        }

        return r;
    }

    /**
     * Iterator methods
     */

    @Override
    public Iterator<TSource> iterator() {
        return iteratorGenerator.apply(this);
    }


    private <TAcc> TAcc evalUnsafeMapIterator(Iterator<TAcc> iterator) {
        iterator.hasNext();

        return iterator.next();
    }

    private TSource unsafeIterEval(Iterator<TSource> iterator) {
        iterator.hasNext();

        return iterator.next();
    }

    private TSource orDefault(Iterator<TSource> iterator){
        if(iterator.hasNext()){
            return iterator.next();
        }

        return null;
    }
}

