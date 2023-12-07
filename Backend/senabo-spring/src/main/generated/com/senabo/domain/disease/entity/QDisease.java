package com.senabo.domain.disease.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QDisease is a Querydsl query type for Disease
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QDisease extends EntityPathBase<Disease> {

    private static final long serialVersionUID = -2119850146L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QDisease disease = new QDisease("disease");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final StringPath diseaseName = createString("diseaseName");

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final com.senabo.domain.member.entity.QMember memberId;

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public QDisease(String variable) {
        this(Disease.class, forVariable(variable), INITS);
    }

    public QDisease(Path<? extends Disease> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QDisease(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QDisease(PathMetadata metadata, PathInits inits) {
        this(Disease.class, metadata, inits);
    }

    public QDisease(Class<? extends Disease> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

