package com.senabo.domain.walk.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QWalk is a Querydsl query type for Walk
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QWalk extends EntityPathBase<Walk> {

    private static final long serialVersionUID = 1509495562L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QWalk walk = new QWalk("walk");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final NumberPath<Double> distance = createNumber("distance", Double.class);

    public final DateTimePath<java.time.LocalDateTime> endTime = createDateTime("endTime", java.time.LocalDateTime.class);

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final com.senabo.domain.member.entity.QMember memberId;

    public final DateTimePath<java.time.LocalDateTime> startTime = createDateTime("startTime", java.time.LocalDateTime.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public QWalk(String variable) {
        this(Walk.class, forVariable(variable), INITS);
    }

    public QWalk(Path<? extends Walk> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QWalk(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QWalk(PathMetadata metadata, PathInits inits) {
        this(Walk.class, metadata, inits);
    }

    public QWalk(Class<? extends Walk> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

